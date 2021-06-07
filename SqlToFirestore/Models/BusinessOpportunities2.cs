using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SqlToFirestore.Models
{
    [FirestoreData]
   public class BusinessOpportunities2
    {
    
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string FullName { get; set; }
        [FirestoreProperty]
        public string Number { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public DateTime Created { get; set; }
        [FirestoreProperty]
        public double Revenue { get; set; }
        [FirestoreProperty]
        public string PipelineText { get; set; }
        [FirestoreProperty]
        public string PipelinePct { get; set; }
        [FirestoreProperty]
        public string QuotaInformation { get; set; }
        [FirestoreProperty]
        public string ReasonLost { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }
        [FirestoreProperty]
        public string Log { get; set; }

        [FirestoreProperty]     
        public string CompanyId { get; set; }     

        [FirestoreProperty]    
        public string ContactId { get; set; }
       
    }
}
