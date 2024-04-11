using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace rosette_api {
     /// <summary>
    /// Parent Interface for RecordSimilarityField objects
    /// </summary>
    public interface RecordSimilarityField
    {

    }
    
    /// <summary>
    /// Abstract parent class for UnfieldedName and FieldedName
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class NameField : RecordSimilarityField
    {
        public const string TEXT = "text";

        /// <summary>
        /// Gets or sets the the name field's text
        /// </summary>
        [JsonProperty(PropertyName = TEXT)]
        public string Text { get; set; }
    }

    /// <summary>
    /// Class for representing an unfielded name
    /// </summary>
    [JsonConverter(typeof(UnfieldedRecordSimilarityConverter))]
    public class UnfieldedNameRecord : NameField
    {

        /// <summary>
        /// No-args constructor
        /// </summary>
        public UnfieldedNameRecord() { }
        
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="text">The name as a string</param>
        public UnfieldedNameRecord(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is UnfieldedNameRecord)
            {
                UnfieldedNameRecord other = obj as UnfieldedNameRecord;
                return this.Text == other.Text;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.Text != null ? this.Text.GetHashCode() : 1;
        }

        /// <summary>
        /// ToString override. Also used for JSON serialization
        /// </summary>
        public override string ToString()
        {
            return this.Text;
        }
    }

    /// <summary> 
    /// Class for representing a fielded name
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class FieldedNameRecord : NameField
    {

        public const string LANGUAGE = "language";
        public const string LANGUAGE_OF_ORIGIN = "languageOfOrigin";
        public const string SCRIPT = "script";
        public const string ENTITY_TYPE = "entityType";

        /// <summary>language
        /// <para>
        /// Getter, Setter for the language
        /// language: Language: ISO 639-3 code
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = LANGUAGE)]
        public string Language { get; set; }

        /// <summary>language
        /// <para>
        /// Getter, Setter for the languageOfOrigin
        /// languageOfOrigin: Language: ISO 639-3 code
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = LANGUAGE_OF_ORIGIN)] 
        public string LanguageOfOrigin { get; set; }

        /// <summary>script
        /// <para>
        /// Getter, Setter for the script
        /// script: ISO 15924 code for the name's script
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = SCRIPT)]
        public string Script { get; set; }

        /// <summary>entityType
        /// <para>
        /// Getter, Setter for the entityType
        /// entityType: Entity type of the name: PERSON, LOCATION, or ORGANIZATION
        /// </para>
        /// </summary>
        [JsonProperty(PropertyName = ENTITY_TYPE)]
        public string EntityType { get; set; }
        
        /// <summary>
        /// No-args constructor
        /// </summary>
        public FieldedNameRecord() { }

        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="text">(string): Text describing the name</param>
        /// <param name="language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="languageOfOrigin">(string, optional): Language the name originates from: ISO 639-3 code (ignored for the /language endpoint)</param>    
        /// <param name="script">(string, optional): ISO 15924 code for the name's script</param>
        /// <param name="entityType">(string, optional): Entity type of the name: PERSON, LOCATION, or ORGANIZATION</param>
        public FieldedNameRecord(string text, string language = null, string languageOfOrigin = null, string script = null, string entityType = null)
        {
            this.Text = text;
            this.Language = language;
            this.LanguageOfOrigin = languageOfOrigin;
            this.Script = script;
            this.EntityType = entityType;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is FieldedNameRecord)
            {
                FieldedNameRecord other = obj as FieldedNameRecord;
                List<bool> conditions = new List<bool>() {
                    this.Text == other.Text,
                    this.Language == other.Language,
                    this.LanguageOfOrigin == other.LanguageOfOrigin,
                    this.Script == other.Script,
                    this.EntityType == other.EntityType
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Text != null ? this.Text.GetHashCode() : 1;
            int h1 = this.Language != null ? this.Language.GetHashCode() : 1;
            int h2 = this.LanguageOfOrigin != null ? this.LanguageOfOrigin.GetHashCode() : 1;
            int h3 = this.Script != null ? this.Script.GetHashCode() : 1;
            int h4 = this.EntityType != null ? this.EntityType.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This fielded name in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Abstract parent class for UnfieldedDate and FieldedDate
    /// </summary>
    public abstract class DateField : RecordSimilarityField
    {
        public const string DATE = "date";

        /// <summary>
        /// Gets or sets the the date field's date
        /// </summary>
        [JsonProperty(PropertyName = DATE)]
        public string Date { get; set; }
    }

    /// <summary>
    /// Class for representing an unfielded date
    /// </summary>
    [JsonConverter(typeof(UnfieldedRecordSimilarityConverter))]
    public class UnfieldedDateRecord : DateField
    {

        /// <summary>
        /// No-args constructor
        /// </summary>
        public UnfieldedDateRecord() { }
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="date">The date in string form</param>
        public UnfieldedDateRecord(string date)
        {
            this.Date = date;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is UnfieldedDateRecord)
            {
                UnfieldedDateRecord other = obj as UnfieldedDateRecord;
                return this.Date == other.Date;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.Date != null ? this.Date.GetHashCode() : 1;
        }

        /// <summary>
        /// ToString override. Also used for JSON serialization
        /// </summary>
        public override string ToString()
        {
            return this.Date;
        }
    }

    /// <summary>
    /// Class for representing a fielded date
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class FieldedDateRecord : DateField
    {

        /// <summary>
        /// No-args constructor
        /// </summary>
        public FieldedDateRecord() { }
        
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="date">The date in string format</param>
        public FieldedDateRecord(string date)
        {
            this.Date = date;
        }
        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is FieldedDateRecord)
            {
                FieldedDateRecord other = obj as FieldedDateRecord;
                List<bool> conditions = new List<bool>() {
                    this.Date == other.Date,
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Date != null ? this.Date.GetHashCode() : 1;
            return h0;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This fielded date in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Abstract parent class for UnfieldedAddress and FieldedAddress
    /// </summary>
    public abstract class AddressField : RecordSimilarityField
    {
        public const string ADDRESS = "address";

        /// <summary>
        /// Gets or sets the the address field's address
        /// </summary>
        [JsonProperty(PropertyName = ADDRESS)]
        public string Address { get; set; }
    }

    /// <summary>
    /// Class for representing an unfielded address
    /// </summary> 
    [JsonConverter(typeof(UnfieldedRecordSimilarityConverter))]
    public class UnfieldedAddressRecord : AddressField
    {
        /// <summary>
        /// No-args constructor
        /// </summary>
        public UnfieldedAddressRecord() { }
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="address">The adress in string form</param>
        public UnfieldedAddressRecord(string address)
        {
            this.Address = address;
        }
        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is UnfieldedAddressRecord)
            {
                UnfieldedAddressRecord other = obj as UnfieldedAddressRecord;
                return this.Address == other.Address;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            return this.Address != null ? this.Address.GetHashCode() : 1;
        }

        /// <summary>
        /// ToString override. Also used for JSON serialization
        /// </summary>
        public override string ToString()
        {
            return this.Address;
        }
    }

    /// <summary>
    /// Class for representing a fielded address
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class FieldedAddressRecord : AddressField
    {
        /// <summary>
        /// No-args constructor
        /// </summary>
        public FieldedAddressRecord() { }
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="address">The adress in string form</param>
        public FieldedAddressRecord(string address)
        {
            this.Address = address;
        }
        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is FieldedAddressRecord)
            {
                FieldedAddressRecord other = obj as FieldedAddressRecord;
                List<bool> conditions = new List<bool>() {
                    this.Address == other.Address,
                };
                return conditions.All(condition => condition);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Hashcode override
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            int h0 = this.Address != null ? this.Address.GetHashCode() : 1;
            return h0;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This fielded address in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}