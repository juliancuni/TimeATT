/*
 * 
    ngaNodeKomanda: {
        "komanda": komanda,
        "userId": userid,
        "emerIPlote": emerIPlote
        "emerIplote": emerIPlote,
        "gishtId": gishtId,
        "privilegji": privilegji,
        "password": password
    }
 *
*/
namespace TimeATT
{
    class KomandaJSONInterface
    {
        public string komanda { get; set; }
        public string attId { get; set; }
        public string emerIplote { get; set; } 
        public string gishtId { get; set; }
        public string privilegji { get; set; }
        public string password { get; set; }
    }
    class PergjigjeJSONInterface
    {
        public string status { get; set; }
        public string reference { get; set; }
        public string attId { get; set; }
        public string gishtId { get; set; }
        public string checkOut { get; set; }
    }
}
