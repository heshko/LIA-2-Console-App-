using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SqlToFirestore.Models
{
    [FirestoreData]
    public  class Activities
    {
        [Key]
        [FirestoreProperty]
        public string ActivitiId { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string FullName { get; set; }
        [FirestoreProperty]
        public DateTime Date { get; set; }
        [FirestoreProperty]
        public string Action { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public string Description { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }
        [FirestoreProperty]
        public string Log { get; set; }
        [FirestoreProperty]
       
        public string SystemIDOrganisation { get; set; }


        [FirestoreProperty]
       
        public string SystemIDPerson { get; set; }
      
    }
}
