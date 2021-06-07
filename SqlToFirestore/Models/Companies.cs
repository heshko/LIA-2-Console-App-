using Google.Cloud.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SqlToFirestore.Models
{
    [FirestoreData]
    public class Companies
    {
     
        [Key]
        [FirestoreProperty]
        public string SystemIDOrganisation { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Organisationsnummer { get; set; }
        [FirestoreProperty]
        public string Telephone { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public string Industry { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
        [FirestoreProperty]
        public string PostCodeZIP { get; set; }
        [FirestoreProperty]
        public string City { get; set; }
        [FirestoreProperty]
        public string Comment { get; set; }
        [FirestoreProperty]
        public string ToolsAndTechnique { get; set; }
        [FirestoreProperty]
        public string Comments { get; set; }
       
        public ICollection<Activities> Activities { get; set; }

      
    }

     

}
