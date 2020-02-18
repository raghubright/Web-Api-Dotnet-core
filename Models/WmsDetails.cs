using System.Collections.Generic;
using System.ComponentModel;
namespace MovejobtoWms.Models
{
    public class WmsDetails
    {

          
        public enum IOPPStages
        {

            PreEditing = 793,
            Firstproof = 795,
            Copyediting = 796,
            Revises1 = 799,
            Revises2 = 800,
            Revises3 = 801,
            Revises4 = 802,
            fromAuthor=803
        }
        public enum BMJStages
        {

            PreEditing = 793,
            Firstproof = 795,
            Copyediting = 796,
            Revises1 = 789,
            Revises2 = 800,
            Revises3 = 801,
            Revises4 = 802,
            Preview = 792,
            XMLValidation = 791
        }
        public enum Wmsids
        {
            BMJ = 36,
            IOPP = 37

        }
        public enum Customers
        {
            BMJCustomer = 12,
            IOPPCustomer = 5
        }
        public enum activities
        {
            IOPPHtmlToPDF = 6131,
            BMJXMLValidation = 6075,
            BMJOnlinePDFCreation = 6080,
            BMJXMLCorrection = 6086,

            HTMLDownload=6144
        }

        public enum WorkFlowIds
        {
            onShoreCopyEditing = 24,
            offShoreCopyEditing = 23,
            authorProofing1 = 25,
            authorProofing2=26,
            bmjProofing1 = 29,
            bmjProofing2 = 30,
            bmjProofing3 = 31,
            bmjProofing4 = 32,
            bmjProofing5 = 33,
            bmjProofing6 = 34,
            bmjProofing7 = 35
          






        }
    }
}