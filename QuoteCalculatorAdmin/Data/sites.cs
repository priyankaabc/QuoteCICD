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
    
    public partial class sites
    {
        public long id { get; set; }
        public string uri_re { get; set; }
        public string name { get; set; }
        public string site_id { get; set; }
        public string company_id { get; set; }
        public string email_domain { get; set; }
        public string quote_subject { get; set; }
        public string booknow_subject { get; set; }
        public string email { get; set; }
        public short branch_needed { get; set; }
        public string branch_default { get; set; }
        public byte[] postcode_sets_branch { get; set; }
        public short show_agent { get; set; }
        public short source_needed { get; set; }
        public string source_default { get; set; }
        public string live_baggage_quote_url { get; set; }
        public string test_baggage_quote_url { get; set; }
        public string dev_baggage_quote_url { get; set; }
        public short needs_alt_quote { get; set; }
        public short use_wizard { get; set; }
        public System.DateTime last_updated { get; set; }
        public long version { get; set; }
    }
}
