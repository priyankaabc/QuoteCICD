using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace QuoteCalculator.Models
{
    public class ShipmentTrackModel
    {
        public string RefrenceNumber { get; set; }
        public string Consignee { get; set; }
    }

    public class ShipmentXMLModel
    {
        public string Message { get; set; }
        public string RefrenceNumber { get; set; }
        public string SurName { get; set; }
        public string ReceivedDate { get; set; }
        public string VesselOrFlightName { get; set; }
        public string ContainerOrAirwaybill { get; set; }
        public string DestinationPort { get; set; }
        public string EstimatedTimeofArrival { get; set; }
        public string DestinationAgentName { get; set; }
        public string AgentTelephone { get; set; }
        public string AgentFax { get; set; }
        public string AgentEmail { get; set; }
        public string Address { get; set; }
    }

    public class Port
    {
        public string adcode { get; set; }
        public string adname { get; set; }
    }

    public class Portloading
    {
        public string adcode { get; set; }
        public string adname { get; set; }
    }

    public class Origin
    {
        public string adcode { get; set; }
        public string adname { get; set; }
        public string adcountry { get; set; }
        public Port port { get; set; }
        public Portloading portloading { get; set; }
    }

    public class Port2
    {
        public string adcode { get; set; }
        public string adname { get; set; }
    }

    public class Portdischarge
    {
        public string adcode { get; set; }
        public string adname { get; set; }
    }

    public class Destination
    {
        public string adcode { get; set; }
        public string adname { get; set; }
        public string adcountry { get; set; }
        public Port2 port { get; set; }
        public Portdischarge portdischarge { get; set; }
    }

    public class Address
    {
        public Origin origin { get; set; }
        public Destination destination { get; set; }
    }

    public class Departure
    {
        public string dddate { get; set; }
    }

    public class Arrival
    {
        public string dddate { get; set; }
    }

    public class Diary
    {
        public Departure departure { get; set; }
        public Arrival arrival { get; set; }
    }

    public class Measurements
    {
        public string meitems { get; set; }
        public string mevolnm { get; set; }
        public string mevolnf { get; set; }
        public string mewgtnl { get; set; }
        public string mewgtnk { get; set; }
        public string mewgtacw { get; set; }
    }

    public class Job
    {
        public string joid { get; set; }
        public string jonumber { get; set; }
        public string joshipped { get; set; }
    }

    public class Entity
    {
        public string enname { get; set; }
    }

    public class Vessel
    {
        public string usreference { get; set; }
        public Entity entity { get; set; }
    }

    public class Address2
    {
        public string adfulladdress { get; set; }
    }

    public class Phone
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Work
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Mobile
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Fax
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Email
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Web
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Contact
    {
        public Phone phone { get; set; }
        public Work work { get; set; }
        public Mobile mobile { get; set; }
        public Fax fax { get; set; }
        public Email email { get; set; }
        public Web web { get; set; }
        public string preferredtime { get; set; }
        public string preferredcontact { get; set; }
    }

    public class Entity2
    {
        public string enid { get; set; }
        public string encode { get; set; }
        public string enname { get; set; }
        public Address2 address { get; set; }
        public Contact contact { get; set; }
    }

    public class Originagent
    {
        public string usreference { get; set; }
        public Entity2 entity { get; set; }
    }

    public class Address3
    {
        public string adfulladdress { get; set; }
    }

    public class Phone2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Work2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Mobile2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Fax2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Email2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Web2
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Contact2
    {
        public Phone2 phone { get; set; }
        public Work2 work { get; set; }
        public Mobile2 mobile { get; set; }
        public Fax2 fax { get; set; }
        public Email2 email { get; set; }
        public Web2 web { get; set; }
        public string preferredtime { get; set; }
        public string preferredcontact { get; set; }
    }

    public class Entity3
    {
        public string enid { get; set; }
        public string encode { get; set; }
        public string enname { get; set; }
        public Address3 address { get; set; }
        public Contact2 contact { get; set; }
    }

    public class Destinationagent
    {
        public string usreference { get; set; }
        public Entity3 entity { get; set; }
    }

    public class Address4
    {
        public string adfulladdress { get; set; }
    }

    public class Entity4
    {
        public string enid { get; set; }
        public string encode { get; set; }
        public string enname { get; set; }
        public Address4 address { get; set; }
    }

    public class Phone3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Work3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Mobile3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Fax3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Email3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Web3
    {
        public string cclabel { get; set; }
        public string cccontact { get; set; }
    }

    public class Contact3
    {
        public Phone3 phone { get; set; }
        public Work3 work { get; set; }
        public Mobile3 mobile { get; set; }
        public Fax3 fax { get; set; }
        public Email3 email { get; set; }
        public Web3 web { get; set; }
        public string preferredtime { get; set; }
        public string preferredcontact { get; set; }
    }

    public class Freightagent
    {
        public string usreference { get; set; }
        public Entity4 entity { get; set; }
        public Contact3 contact { get; set; }
    }

    public class Entity5
    {
        public string endesc { get; set; }
        public string ensize { get; set; }
        public string encode { get; set; }
    }

    public class Container
    {
        public string usreference { get; set; }
        public Entity5 entity { get; set; }
    }

    public class Usage
    {
        public Vessel vessel { get; set; }
        public Originagent originagent { get; set; }
        public Destinationagent destinationagent { get; set; }
        public Freightagent freightagent { get; set; }
        public Container container { get; set; }
    }

    public class RootObject
    {
        public string __PK { get; set; }
        public string _mode { get; set; }
        public string joid { get; set; }
        public string jonumber { get; set; }
        public string jojobtype { get; set; }
        public string joservice { get; set; }
        public string jodetail { get; set; }
        public string jofile { get; set; }
        public string jojobstatus { get; set; }
        public Address address { get; set; }
        public Diary diary { get; set; }
        public Measurements measurements { get; set; }
        public Job job { get; set; }
        public Usage usage { get; set; }
    }
}