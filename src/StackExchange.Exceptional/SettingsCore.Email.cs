#if COREFX

namespace StackExchange.Exceptional
{
    public partial class Settings
    {
        /// <summary>
        /// The ErrorStore section of the configuration, optional and will default to a MemoryErrorStore if not specified
        /// </summary>
        public EmailSettingsConfig Email { get; set; }
    }


    /// <summary>
    /// Interface for email settings, either direct or from a config
    /// </summary>
    public interface IEmailSettings
    {
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="ToAddress"]/*' />
        string ToAddress { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromAddress"]/*' />
        string FromAddress { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromDisplayName"]/*' />
        string FromDisplayName { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPHost"]/*' />
        string SMTPHost { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPort"]/*' />
        /// 
        int SMTPPort { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPUserName"]/*' />
        string SMTPUserName { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPassword"]/*' />
        string SMTPPassword { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPEnableSSL"]/*' />
        bool SMTPEnableSSL { get; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="PreventDuplicates"]/*' />
        bool PreventDuplicates { get; }
    }
    
    /// <summary>
    /// Email settings configuration, for configuring Email sending from code
    /// </summary>
    public class EmailSettings : IEmailSettings
    {
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="ToAddress"]/*' />
        public string ToAddress { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromAddress"]/*' />
        public string FromAddress { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromDisplayName"]/*' />
        public string FromDisplayName { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPHost"]/*' />
        public string SMTPHost { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPort"]/*' />
        public int SMTPPort { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPUserName"]/*' />
        public string SMTPUserName { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPassword"]/*' />
        public string SMTPPassword { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPEnableSSL"]/*' />
        public bool SMTPEnableSSL { get; set; }
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="PreventDuplicates"]/*' />
        public bool PreventDuplicates { get; set; }

        /// <summary>
        /// Creates an email settings object defaulting the SMTP port to 25
        /// </summary>
        public EmailSettings()
        {
            SMTPPort = 25;
        }
    }

    /// <summary>
    /// A settings object describing email properties
    /// </summary>
    public class EmailSettingsConfig : IEmailSettings
    {
        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="ToAddress"]/*' />
        public string ToAddress { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromAddress"]/*' />
        public string FromAddress { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="FromDisplayName"]/*' />
        public string FromDisplayName { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPHost"]/*' />
        public string SMTPHost { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPort"]/*' />
        public int SMTPPort { get; set; } = 25;

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPUserName"]/*' />
        public string SMTPUserName { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPPassword"]/*' />
        public string SMTPPassword { get; set; }

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="SMTPEnableSSL"]/*' />
        public bool SMTPEnableSSL { get; set; } = false;

        /// <include file='SharedDocs.xml' path='SharedDocs/IEmailSettings/Member[@name="PreventDuplicates"]/*' />
        public bool PreventDuplicates { get; set; } = false;
    }
}
#endif