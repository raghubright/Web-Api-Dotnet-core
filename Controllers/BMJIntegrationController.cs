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
namespace BMJIntegration.Controllers
{
    // [Authorize(AuthenticationSchemes = "CustomAuthentication")]
    [ApiController]
    [Route("[controller]/{action}")]
    public class BMJIntegrationController : ControllerBase
    {
        private readonly MoveJobContext _context;
        private readonly IConfiguration _configuration;

        public BMJIntegrationController(MoveJobContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //To update the status
        public string UpdateArticle(string Articlename, string ZipFile)
        {
            try
            {
                string sException = string.Empty;
                IList<SP_BMJ_JobDetails> _result = _context.SP_BMJ_JobDetails.FromSqlRaw("exec SP_BMJ_JobDetails  '" + Articlename + "'").ToList();
                if (_result.Count == 0)
                {
                    sException = _configuration["BMJIntegration:JobDetailsMissing"];
                }
                else
                {
                    if (System.IO.File.Exists(ZipFile))
                    {
                        FileInfo fileInfo = new FileInfo(ZipFile);
                        var client = new RestClient(_configuration["BMJIntegration:host"]);
                        var request = new RestRequest(_configuration["BMJIntegration:method"], Method.POST);
                        request.AddHeader("apikey", _configuration["BMJIntegration:apikey"]);
                        request.AddHeader("clientid", _configuration["BMJIntegration:clientid"]);
                        request.AddFile("file", ZipFile);
                        request.AddParameter("JournalName", _result[0].JournalName.Trim());
                        request.AddParameter("JournalCode", _result[0].JournalCode.Trim());
                        request.AddParameter("CustId", 12);
                        request.AddParameter("ArticleName", _result[0].ArticleName.Trim());
                        request.AddParameter("ArticleCode", _result[0].ArticleCode.Trim());
                        request.AddParameter("ArticleTitle",_result[0].ArticleTitle.Trim());
                        request.AddParameter("size", fileInfo.Length);
                        request.AddParameter("PEUserName", _result[0].PEUserName.Trim());
                        request.AddParameter("PEUserEmail", _result[0].PEUserEmail.Trim());
                        request.AddParameter("PMUserName", _result[0].PMUserName.Trim());
                        request.AddParameter("PMUserEmail", _result[0].PMUserEmail.Trim());
                        request.AlwaysMultipartFormData = true;
                        IRestResponse response = client.Execute(request);
                        if (response.StatusCode == 0 || response.StatusDescription == "Unauthorized")
                        {
                            sException = response.ErrorMessage;
                        }
                        else
                        {
                            sException = response.Content.ToString();
                        }
                    }
                    else
                    {
                        sException = _configuration["BMJIntegration:FileMissing"];
                    }
                }
                return sException;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //To Upload the files customer zip package

        public string UpdateArticleStatus(string Articlename, int StageId, int ActivityId, int SeqId)
        {
            try
            {

                BMJ_MstCustomerStageDetails _resultCustomerStage = _context.BMJ_MstCustomerStageDetails.Where(r => r.WMSActivityId == ActivityId && r.WMSStageId == StageId && r.SeqId == SeqId).FirstOrDefault();
                if (_resultCustomerStage == null)
                {
                    return _configuration["BMJIntegration:StageMissing"];
                }

                IList<SP_BMJ_AssignedJobDetails> _resultAssingJob = _context.SP_BMJ_AssignedJobDetails.FromSqlRaw("exec SP_BMJ_AssignedJobDetails  '" + Articlename + "'").ToList();
                if (_resultAssingJob.Count == 0)
                {
                    return _configuration["BMJIntegration:JobDetailsMissing"];
                }

                string sException = string.Empty;
                string Bearertoken = GenerateToken();
                if (Bearertoken.Length < 2) throw (new Exception(_configuration["BMJIntegration:InvalidToken"]));
                Bearertoken = "Bearer " + Bearertoken.Substring(1, Bearertoken.Length - 2);
                var client = new RestClient(_configuration["BMJIntegration:UpdateArticleStatus"]);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", Bearertoken);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "{\n    \"ManuscriptID\":\"" + _resultAssingJob[0].bookcode + "\",\n    \"DOI\": \"" + _resultAssingJob[0].DOI_Number + "\",\n    \"ArticleTitle\":\"" + _resultAssingJob[0].ArticleTitle  + "\",\n    \"ArticleType\":\"" + _resultAssingJob[0].ArticleType + "\",\n    \"ArticleID\":\"" + _resultAssingJob[0].ArticleId + "\",\n    \"AuthorName\": \"" + _resultAssingJob[0].Author + "\",\n    \"ArticleCode\":\"" + _resultAssingJob[0].bookcode + "\",\n    \"License\": \"" + _resultAssingJob[0].Licence + "\",\n    \"CustID\":12,\n    \"CurrentStage\":\"" + _resultCustomerStage.CustomerStage + "\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == 0 || response.StatusDescription == "Unauthorized")
                {
                    sException = response.ErrorMessage;
                }
                else
                {
                    dynamic json = JsonConvert.DeserializeObject(response.Content);
                    sException = JsonConvert.SerializeObject(json.message.ToString());
                    if (sException.Length > 2) sException = sException.Substring(1, sException.Length - 2);
                }
                return sException;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        public string SupplementaryUpload(int ICMSIntegrationID, int CMSActivityID, string File, string Flag)
        {
            try
            {
                string sException = string.Empty;
                if (System.IO.File.Exists(File))
                {
                    if(!File.Contains("\\")) return _configuration["BMJIntegration:InvalidPath"];
                    string Bearertoken = GenerateToken();
                    if (Bearertoken.Length < 2) throw (new Exception(_configuration["BMJIntegration:InvalidToken"]));
                    Bearertoken = "Bearer " + Bearertoken.Substring(1, Bearertoken.Length - 2);
                    FileInfo fileInfo = new FileInfo(File);
                    var client = new RestClient(_configuration["BMJIntegration:host"]);
                    var request = new RestRequest(_configuration["BMJIntegration:supplement"], Method.POST);
                    request.AddHeader("Authorization", Bearertoken);
                    request.AddParameter("flag", Flag);
                    request.AddParameter("wfd_id", CMSActivityID);
                    request.AddParameter("Article_GUID", ICMSIntegrationID);
                    request.AddFile("file", File);
                    request.AlwaysMultipartFormData = true;
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == 0 || response.StatusDescription == "Unauthorized")
                    {
                        sException = response.ErrorMessage;
                    }
                    else
                    {
                        dynamic json = JsonConvert.DeserializeObject(response.Content);
                        sException = JsonConvert.SerializeObject(json.message.ToString());
                        if (sException.Length > 2) sException = sException.Substring(1, sException.Length - 2);
                    }
                }
                else
                {
                    sException = _configuration["BMJIntegration:FileMissing"];
                }

                return sException;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        //To 
        [NonAction]
        public string GenerateToken()
        {
            try
            {
                var client = new RestClient(_configuration["BMJIntegration:login"]);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("apikey", _configuration["BMJIntegration:apikey"]);
                request.AddHeader("clientid", _configuration["BMJIntegration:clientid"]);
                request.AddParameter("application/json", "{\r\n    \"UserName\":\"admin@integra.co.in\",\r\n    \"FirstName\":\"admin@integra.co.in\",\r\n    \"RoleId\":\"4\",\r\n    \"EmailID\":\"admin@integra.co.in\"\r\n}\r\n", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                dynamic json = JsonConvert.DeserializeObject(response.Content);
                return JsonConvert.SerializeObject(json.token);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }

}