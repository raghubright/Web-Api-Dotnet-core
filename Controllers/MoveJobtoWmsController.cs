using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.Metadata;
using System.Text;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovejobtoWms.Models;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text.Json;
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

using static MovejobtoWms.Models.WmsDetails;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.Emit;

namespace MovejobtoWms.Controllers
{
    // [Authorize(AuthenticationSchemes = "CustomAuthentication")]
    [Route("api/[controller]/{action}")]
    [ApiController]
    public class MoveJobtoWmsController : ControllerBase
    {

        HttpClient client = new HttpClient();
        List<int> bmjworkflowids = ((WorkFlowIds[])Enum.GetValues(typeof(WorkFlowIds))).Select(c => (int)c).ToList();
        int activityid = 0;
        int stageid = 0;
        private readonly MoveJobContext _context;

        // IEnumerable<int> Worflowids = new List<int>(Enumerable.Range(29, 7));
        //private IUserService _userService;
        public IConfiguration _configuration { get; }
        private readonly IWebHostEnvironment _env;
        public MoveJobtoWmsController(MoveJobContext context, IConfiguration configuration, IWebHostEnvironment env)
        {


            _configuration = configuration;
            _context = context;
            _env = env;
            // _configuration = configuration;

            //   _userService = userService;

        }


        // POST: api/MoveJobtoWms/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpGet]
        public String TestMessage()
        {
            return "Welcome to Movejobtowms Api I am Working Fine! for evn : " + _env.EnvironmentName;
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(201)]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]

        public async Task<IActionResult> UpdatingActivityDetail([FromBody] JsonElement id)
        {
            string DownloadDirectory = _configuration["Configuration:DownloadPath"].Replace("/", "\\");
            int Status = 0;
            string JournalName = string.Empty;
            string filename = string.Empty;
            var responsedetail = new Response();
            string sCaptureError = "";
            var fileDetail = new FileDetail();

            // client.BaseAddress = new Uri(_configuration["Configuration:Uri"]);
            // client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(
            //  new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.Add("clientid", _configuration["Configuration:ClientId"]);
            // client.DefaultRequestHeaders.Add("apikey", _configuration["Configuration:apiKey"]);
            var data = id.ToString();

            JObject jObject = JObject.Parse(data);
            string value = (string)jObject.SelectToken("id");
            var ids = "{\n\t\"ids\":[" + value + "]\n}".ToString();
            // var response = client.PostAsync(_configuration["Configuration:GetActivityFiles"], new StringContent(data, Encoding.UTF8, "application/json")).Result;

            var client = new RestClient(_configuration["Configuration:LiveUri"] + _configuration["Configuration:GetActivityFiles"]);
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("clientid", _configuration["Configuration:ClientId"]);
            request.AddHeader("apikey", _configuration["Configuration:apiKey"]);
            request.AddParameter("application/json", data, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                responsedetail = JsonConvert.DeserializeObject<Response>(response.Content.ToString());
            }

            if (responsedetail.is_success)
            {

                //updating Status to Trn File Property Details
                Status = responsedetail.data.Status;
                JournalName = responsedetail.data.JournalName;
                filename = responsedetail.data.ArticleName;

                var bookdetails = _context.BookDetails.FirstOrDefault(x => x.bookName == JournalName);
                fileDetail = _context.FileDetails.FirstOrDefault(x => x.fileName == filename);

                try
                {

                    //  var fileDetail = await _context.FileDetails.FirstOrDefaultAsync(x => x.fileName == filename);
                    if (fileDetail == null)
                    {
                        return StatusCode(200, new
                        {
                            is_success = false,
                            data = "",
                            message = string.Format("File Details Not exist for given Articlname {0} ", filename)
                        });
                    }
                    else if (fileDetail != null)
                    {

                        var FileProplold = await _context.trnFilePropertyDetails.FirstOrDefaultAsync(x => x.fileId == fileDetail.fileID && x.bookId == bookdetails.bookId && x.id == 195);


                        if (FileProplold != null)
                        {

                            try
                            {

                                FileProplold.value = Status.ToString();
                                //  _context.Recorddetails.Attach(recordetailold);
                                //  _context.Entry(recordetailold).Property(x=>x.status & x.bookid &x.fileID).IsModified=true;
                                //  _context.SaveChanges();
                                _context.trnFilePropertyDetails.Update(FileProplold);

                                await _context.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {

                                return StatusCode(200, new
                                {
                                    is_success = false,
                                    data = "",
                                    message = "Error in Updating FilePropertDetails Error Desc: " + ex.Message.ToString()
                                });
                            }
                        }
                        else
                        {

                            var recordetail = new trnFilePropertyDetail
                            {
                                // id = 195, //Customer Category file Property
                                fileId = fileDetail.fileID,
                                bookId = bookdetails.bookId,
                                value = Status.ToString()

                            };
                            try
                            {
                                await _context.AddAsync(recordetail);
                                await _context.SaveChangesAsync();

                            }
                            catch (Exception ex)
                            {

                                return StatusCode(200, new
                                {
                                    is_success = false,
                                    data = "",
                                    message = "Error in Inserting FilePropertDetails  Error Desc: " + ex.Message.ToString()
                                });

                            }
                        }

                    }


                    //For CustomerID IOPP from Iatuhor
                    if (responsedetail.data.CustomerId == Customers.IOPPCustomer)

                    {

                        if (responsedetail.data.WorkflowId == (int)WorkFlowIds.authorProofing2 && Status == 3)
                        {
                            //adding Activity and Stage Detail for abovedotne Workflow ID's Enabling HTML Download Acitivity in Copy Editing Stage
                            stageid = (int)IOPPStages.fromAuthor;
                            activityid = (int)activities.HTMLDownload;
                        }
                        else
                        {
                            //adding Activity and Stage Detail for abovedotne Workflow ID's Enabling Pdf Creation Acitivity in Copy Editing Stage
                            stageid = (int)IOPPStages.Copyediting;
                            activityid = (int)activities.IOPPHtmlToPDF;
                        }
                        try
                        {

                            if (fileDetail != null)
                            {
                                var recordetailold = await _context.Recorddetails37.FirstOrDefaultAsync(x => x.fileID == fileDetail.fileID && x.activityid == activityid);

                                if (recordetailold != null)
                                {

                                    try
                                    {
                                        if (recordetailold.status == 0)
                                        {

                                            recordetailold.status = 1;
                                            _context.Recorddetails37.Update(recordetailold);
                                            await _context.SaveChangesAsync();
                                        }
                                        else
                                        {

                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        return StatusCode(200, new
                                        {
                                            is_success = false,
                                            data = "",
                                            message = "Error in Updating Activity Details  Error Desc: " + ex.Message.ToString()
                                        });
                                    }
                                }
                                else
                                {
                                    var recordetail = new Recorddetail37
                                    {
                                        fileID = fileDetail.fileID,
                                        activityid = (Int16)activityid,
                                        stageId = (Int16)stageid,
                                        bookid = (Int16)fileDetail.bookid,

                                    };
                                    try
                                    {
                                        await _context.AddAsync(recordetail);
                                        await _context.SaveChangesAsync();

                                    }
                                    catch (Exception ex)
                                    {

                                        return StatusCode(200, new
                                        {
                                            is_success = false,
                                            data = "",
                                            message = "Error in Inserting Activity Details  Error Desc: " + ex.Message.ToString()
                                        });

                                    }
                                }

                            }


                        }
                        catch (Exception ex)
                        {

                            return StatusCode(200, new
                            {
                                is_success = false,
                                data = "",
                                message = ex.Message.ToString()
                            });
                        }



                    }
                    //For CustomerID BMJ from Iatuhor
                    else if (responsedetail.data.CustomerId == Customers.BMJCustomer)
                    {
                        try
                        {

                            ActivityDetail activityDetail = new ActivityDetail();
                            StageDetail StageDetail = new StageDetail();
                            string FileName = Path.GetFileName(responsedetail.data.path.ToString());
                            DownloadDirectory = DownloadDirectory.Replace(";bookname;", responsedetail.data.JournalName.ToString());
                            String DownloadPath = DownloadDirectory + filename + "\\" + FileName;
                            int WorkflowId = responsedetail.data.WfdId;
                            string ArticleName = responsedetail.data.ArticleName;
                            Status = responsedetail.data.Status;
                            string UriPath = responsedetail.data.path.ToString();
                            bool IsitrackStageAdded = false;
                            // string pdfName = Path.GetFileName(jobinputs.pdfPath);
                            // string xmlFName = Path.GetFileName(jobinputs.xmlPath);
                            // string appFName = Path.GetFileName(jobinputs.ApplicationPath);


                            // if (jobinputs.Wfdid == "23" || jobinputs.Wfdid == "24")
                            if (bmjworkflowids.Contains(WorkflowId))
                            {

                                if (bmjworkflowids.Contains(WorkflowId) && Status == 17)
                                {

                                    //adding Activity and Stage Detail for abovedotne Workflow ID's
                                    stageid = (int)BMJStages.Revises1; //
                                    activityid = (int)activities.BMJXMLCorrection;
                                    //dynamically Create folder structure based on 
                                    activityDetail = _context.ActivityDetails.FirstOrDefault(x => x.activityid == activityid);
                                    StageDetail = _context.StageDetails.FirstOrDefault(x => x.stageid == stageid);
                                    DownloadPath = DownloadPath.Replace(";stageDetail;", StageDetail.sfoldername);
                                    DownloadPath = DownloadPath.Replace(";ActivityDetail;", activityDetail.folderName);

                                    IsitrackStageAdded = AddtoItrackStage(ArticleName, "", 128, 513, 3, 1, ref sCaptureError);

                                }
                                else if (bmjworkflowids.Contains(WorkflowId) && Status == 3)
                                {

                                    stageid = (int)BMJStages.Preview;
                                    activityid = (int)activities.BMJOnlinePDFCreation;

                                    activityDetail = await _context.ActivityDetails.FirstAsync(x => x.activityid == activityid);
                                    StageDetail = _context.StageDetails.FirstOrDefault(x => x.stageid == stageid);
                                    DownloadPath = DownloadPath.Replace(";stageDetail;", StageDetail.sfoldername);
                                    DownloadPath = DownloadPath.Replace(";ActivityDetail;", activityDetail.folderName);
                                    IsitrackStageAdded = AddtoItrackStage(ArticleName, "", 128, 513, 63, 1, ref sCaptureError);
                                }
                                else
                                {
                                    stageid = (int)BMJStages.XMLValidation;
                                    activityid = (int)activities.BMJXMLValidation;
                                    activityDetail = _context.ActivityDetails.FirstOrDefault(x => x.activityid == activityid);
                                    StageDetail = _context.StageDetails.FirstOrDefault(x => x.stageid == stageid);
                                    DownloadPath = DownloadPath.Replace(";stageDetail;", StageDetail.sfoldername);
                                    DownloadPath = DownloadPath.Replace(";ActivityDetail;", activityDetail.folderName);
                                }

                                if (!string.IsNullOrEmpty(UriPath))
                                {

                                    if ((bmjworkflowids.Contains(WorkflowId)))
                                    {

                                        try
                                        {

                                            fileDetail = await _context.FileDetails.FirstOrDefaultAsync(x => x.fileName == filename);


                                            if (Decompress(DownloadPath, Path.GetDirectoryName(DownloadPath), UriPath, ref sCaptureError))
                                            {

                                                if (fileDetail != null)
                                                {
                                                    var recordetailold = await _context.Recorddetails.FirstOrDefaultAsync(x => x.fileID == fileDetail.fileID && x.activityid == activityid);


                                                    if (recordetailold != null)
                                                    {

                                                        try
                                                        {

                                                            if (recordetailold.status == 0)
                                                            {

                                                                recordetailold.status = 1;
                                                                //  _context.Recorddetails.Attach(recordetailold);
                                                                //  _context.Entry(recordetailold).Property(x=>x.status & x.bookid &x.fileID).IsModified=true;
                                                                //  _context.SaveChanges();
                                                                _context.Recorddetails.Update(recordetailold);

                                                                await _context.SaveChangesAsync();
                                                            }
                                                            else
                                                            {

                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                            return StatusCode(200, new
                                                            {
                                                                is_success = false,
                                                                data = "",
                                                                message = "Error in Updating Activity Details Error Desc: " + ex.Message.ToString()
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var recordetail = new Recorddetail
                                                        {
                                                            fileID = fileDetail.fileID,
                                                            activityid = (Int16)activityid,
                                                            stageId = (Int16)stageid,
                                                            bookid = (Int16)fileDetail.bookid


                                                        };
                                                        try
                                                        {
                                                            await _context.AddAsync(recordetail);
                                                            await _context.SaveChangesAsync();

                                                        }
                                                        catch (Exception ex)
                                                        {

                                                            return StatusCode(200, new
                                                            {
                                                                is_success = false,
                                                                data = "",
                                                                message = "Error in Inserting Activity Details  Error Desc: " + ex.Message.ToString()
                                                            });

                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return StatusCode(200, new
                                                {
                                                    is_success = false,
                                                    data = "",
                                                    message = sCaptureError
                                                });
                                            }


                                        }
                                        catch (Exception ex)
                                        {

                                            return StatusCode(200, new
                                            {
                                                is_success = false,
                                                data = "",
                                                message = ex.Message.ToString()
                                            });
                                        }
                                    }
                                    else
                                    {
                                        return StatusCode(200, new
                                        {
                                            is_success = false,
                                            data = "",
                                            message = ""
                                        });
                                    }
                                }
                                else
                                {

                                    return StatusCode(200, new
                                    {
                                        is_success = false,
                                        data = "",
                                        message = "File Path Should not be empty"
                                    });

                                }
                            }
                            else
                            {
                                return StatusCode(200, new
                                {
                                    is_success = false,
                                    data = "",
                                    message = " workflow id is not configured for id: " + WorkflowId
                                });
                            }

                        }
                        catch (Exception ex)
                        {
                            return StatusCode(200, new
                            {
                                is_success = false,
                                data = "",
                                message = ex.Message.ToString()
                            });
                        }
                    }



                }
                catch (Exception ex)
                {

                    return StatusCode(200, new
                    {
                        is_success = false,
                        data = "",
                        message = ex.Message.ToString()
                    });
                }

                var clientupdate = new RestClient(_configuration["Configuration:Uri"] + _configuration["Configuration:GetActivityFiles"]);
                var requestupdate = new RestRequest(Method.POST);
                requestupdate.AddHeader("content-type", "application/json");
                requestupdate.AddHeader("clientid", _configuration["Configuration:ClientId"]);
                requestupdate.AddHeader("apikey", _configuration["Configuration:apiKey"]);
                requestupdate.AddParameter("application/json", ids, ParameterType.RequestBody);
                IRestResponse responsenew = client.Execute(request);

                if (responsenew.IsSuccessful)
                {

                    responsedetail = JsonConvert.DeserializeObject<Response>(response.Content.ToString());
                    if (responsedetail.is_success)
                    {
                        return StatusCode(200, new
                        {
                            is_success = true,
                            data = "",
                            message = "Success"
                        });
                    }
                    else
                    {
                        return StatusCode(200, new
                        {
                            is_success = true,
                            data = "",
                            message = "Success"
                        });
                    }
                }
                else
                {
                    return StatusCode(200, new
                    {
                        is_success = false,
                        data = "",
                        message = "Error in Updating Status"
                    });
                }
            }
            else
            {
                return StatusCode(200, new
                {
                    is_success = false,
                    data = "",
                    message = "No Respond from IAuthor URL "
                });
            }

        }

        [NonAction]
        private bool AddtoItrackStage(string BookCode, string Remark, int DivisionID, int CustomerID, int StageID, int MsPage, ref string sError)
        {

            try
            {
                var con = _context.Database.GetDbConnection();
                var cmd = con.CreateCommand();
                cmd.CommandText = "SELECT GETDATE()";
                con.Open();
                var datetime = (DateTime)cmd.ExecuteScalar();
                
                con.Close();


                var client = new RestClient(_configuration["Configuration:ItrackApiUrl"]);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("undefined", "BookCode=" + BookCode + "&DivisionId=" + DivisionID + "&CustomerId=" + CustomerID + "&StageId=" + StageID + "&ReceiveDate=" + datetime.ToString("MM-dd-yyyy h:m:s") + "&DueDate=" + datetime.AddHours(24).ToString("MM-dd-yyyy h:m:s"), ParameterType.RequestBody);
                string sResponse = client.Post(request).Content.ToString();
                if (sResponse.ToLower().Contains("false"))
                {
                    throw (new Exception(sResponse));
                }
                else
                {
                    return true;
                }


            }
            catch (Exception ex)
            {
                sError = ex.Message.ToString();
                return false;
            }
        }
        [NonAction]
        private static bool Decompress(string zipFilePath, string extractPath, string uriPath, ref string sError)
        {
            WebClient client = new WebClient();
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(zipFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(zipFilePath));

                client.DownloadFile(new Uri(uriPath), zipFilePath);
                // using ( objImpersonate = new ImpersonationService("itoolsusers", "Integra-India.com", "hIe6arCh7"))
                // SafeAccessTokenHandle safeAccessTokenHandle;
                // bool returnValue = LogonUser("itoolsusers", "INTEGRA-INDIA","hIe6arCh7",
                // LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                // out safeAccessTokenHandle);

                if (System.IO.File.Exists(zipFilePath) == false)
                {
                    ///sError = zipFilePath + " does not exists";
                    // return false;
                }
                if (Directory.Exists(extractPath) == false)
                {
                    Directory.CreateDirectory(extractPath);
                }

                //not over write
                //ZipFile.ExtractToDirectory(zipFilePath, extractPath);

                //over write
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string ename = string.Empty;
                        if (entry.FullName.EndsWith("/"))
                        {
                            ename = entry.FullName.Substring(0, entry.FullName.Length - 1);
                        }
                        else
                        {
                            ename = entry.FullName;
                        }
                        string FullPath = Path.Combine(extractPath, ename.Replace("/", "\\"));
                        FullPath = Path.Combine(extractPath, Path.GetFileName(FullPath));
                        if (Regex.IsMatch(FullPath, "\\.[a-z]{1,}", RegexOptions.IgnoreCase))
                        {
                            entry.ExtractToFile(FullPath, true);
                        }
                        else
                        {
                            //if (!Directory.Exists(FullPath))
                            //{
                            //    Directory.CreateDirectory(FullPath);
                            //}
                        }
                    }
                }

                // }
                if (System.IO.File.Exists(zipFilePath))
                    System.IO.File.Delete(zipFilePath);
                return true;



            }
            catch (Exception ex)
            {
                sError = ex.Message.ToString();
                return false;
            }
        }

        private bool FileDetailExists(int id)
        {
            return _context.FileDetails.Any(e => e.fileID == id);
        }
    }
}