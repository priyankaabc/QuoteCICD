//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuoteCalculatorAdmin.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class job_registrations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public job_registrations()
        {
            this.job_agency_email_log = new HashSet<job_agency_email_log>();
        }
    
        public long id { get; set; }
        public long job_industry_id { get; set; }
        public long job_type_id { get; set; }
        public long job_position_id { get; set; }
        public long job_residency_id { get; set; }
        public long nationality_id { get; set; }
        public string forename { get; set; }
        public string surname { get; set; }
        public string contact_number { get; set; }
        public string email { get; set; }
        public string current_employer { get; set; }
        public string criminal_convictions { get; set; }
        public Nullable<long> cover_letter_id { get; set; }
        public Nullable<long> cv_id { get; set; }
        public string message { get; set; }
        public string position_sought { get; set; }
        public System.DateTime intended_arrival { get; set; }
        public System.DateTime intended_work { get; set; }
        public long job_country_id { get; set; }
        public Nullable<long> job_area_id { get; set; }
        public Nullable<long> job_city_id { get; set; }
        public Nullable<System.DateTime> submission { get; set; }
        public Nullable<System.DateTime> registrant_email_sent { get; set; }
        public Nullable<System.DateTime> agency_email_sent { get; set; }
        public Nullable<System.DateTime> followup_email_sent { get; set; }
        public string current_position { get; set; }
        public string current_salary { get; set; }
        public string education_qualification { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string postcode { get; set; }
        public short info_baggage_shipping { get; set; }
        public short info_banking_downunder { get; set; }
        public short info_car_shipping { get; set; }
        public short info_international_removals { get; set; }
        public short info_money_transfers { get; set; }
        public short info_pet_transportation { get; set; }
        public short info_uk_tax_rebates { get; set; }
        public short info_pension_transfers { get; set; }
        public short info_emigration_visas { get; set; }
    
        public virtual carea carea { get; set; }
        public virtual city city1 { get; set; }
        public virtual countries countries { get; set; }
        public virtual countries countries1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<job_agency_email_log> job_agency_email_log { get; set; }
        public virtual job_industry job_industry { get; set; }
        public virtual job_position job_position { get; set; }
        public virtual job_registrations_documents job_registrations_documents { get; set; }
        public virtual job_registrations_documents job_registrations_documents1 { get; set; }
        public virtual job_residence job_residence { get; set; }
        public virtual job_type job_type { get; set; }
    }
}
