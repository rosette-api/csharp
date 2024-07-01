using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public const string FORMAT = "format";

        /// <summary>
        /// Gets or sets the the date field's format
        /// </summary>
        [JsonProperty(PropertyName = FORMAT)]
        public string Format { get; set; }

        /// <summary>
        /// No-args constructor
        /// </summary>
        public FieldedDateRecord() { }
        
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="date">The date in string format</param>
        /// <param name="format">The date's format. Rules are defined at https://docs.oracle.com/javase/8/docs/api/java/time/format/DateTimeFormatter.html</param>
        public FieldedDateRecord(string date, string format)
        {
            this.Date = date;
            this.Format = format;
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
                    this.Format == other.Format
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
            int h1 = this.Format != null ? this.Format.GetHashCode() : 1;
            return h0 ^ h1;
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

    }

    /// <summary>
    /// Class for representing an unfielded address
    /// </summary> 
    [JsonConverter(typeof(UnfieldedRecordSimilarityConverter))]
    public class UnfieldedAddressRecord : AddressField
    {

        public const string ADDRESS = "address";

        /// <summary>
        /// Gets or sets the the address field's address
        /// </summary>
        [JsonProperty(PropertyName = ADDRESS)]
        public string Address { get; set; }

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
        public const string HOUSE = "house";
        public const string HOUSE_NUMBER = "houseNumber";
        public const string ROAD = "road";
        public const string UNIT = "unit";
        public const string LEVEL = "level";
        public const string STAIRCASE = "staircase";
        public const string ENTRANCE = "entrance";
        public const string SUBURB = "suburb";
        public const string CITY_DISTRICT = "cityDistrict";
        public const string CITY = "city";
        public const string ISLAND = "island";
        public const string STATE_DISTRICT = "stateDistrict";
        public const string STATE = "state";
        public const string COUNTRY_REGION = "countryRegion";
        public const string COUNTRY = "country";
        public const string WORLD_REGION = "worldRegion";
        public const string POSTCODE = "postcode";
        public const string PO_BOX = "poBox";

        /// <summary>
        /// Gets or sets the the address field's house
        /// </summary>
        [JsonProperty(PropertyName = HOUSE)]
        public string House { get; set; }

        /// <summary>
        /// Gets or sets the the address field's house number
        /// </summary>
        [JsonProperty(PropertyName = HOUSE_NUMBER)]
        public string HouseNumber { get; set; }

        /// <summary>
        /// Gets or sets the the address field's road
        /// </summary>
        [JsonProperty(PropertyName = ROAD)]
        public string Road { get; set; }

        /// <summary>
        /// Gets or sets the the address field's unit
        /// </summary>
        [JsonProperty(PropertyName = UNIT)]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the the address field's level
        /// </summary>
        [JsonProperty(PropertyName = LEVEL)]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the the address field's staircase
        /// </summary>
        [JsonProperty(PropertyName = STAIRCASE)]
        public string Staircase { get; set; }

        /// <summary>
        /// Gets or sets the the address field's entrance
        /// </summary>
        [JsonProperty(PropertyName = ENTRANCE)]
        public string Entrance { get; set; }

        /// <summary>
        /// Gets or sets the the address field's suburb
        /// </summary>
        [JsonProperty(PropertyName = SUBURB)]
        public string Suburb { get; set; }

        /// <summary>
        /// Gets or sets the the address field's city district
        /// </summary>
        [JsonProperty(PropertyName = CITY_DISTRICT)]
        public string CityDistrict { get; set; }

        /// <summary>
        /// Gets or sets the the address field's city
        /// </summary>
        [JsonProperty(PropertyName = CITY)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the the address field's island
        /// </summary>
        [JsonProperty(PropertyName = ISLAND)]
        public string Island { get; set; }

        /// <summary>
        /// Gets or sets the the address field's state district
        /// </summary>
        [JsonProperty(PropertyName = STATE_DISTRICT)]
        public string StateDistrict { get; set; }

        /// <summary>
        /// Gets or sets the the address field's state
        /// </summary>
        [JsonProperty(PropertyName = STATE)]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the the address field's country region
        /// </summary>
        [JsonProperty(PropertyName = COUNTRY_REGION)]
        public string CountryRegion { get; set; }

        /// <summary>
        /// Gets or sets the the address field's country
        /// </summary>
        [JsonProperty(PropertyName = COUNTRY)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the the address field's world region
        /// </summary>
        [JsonProperty(PropertyName = WORLD_REGION)]
        public string WorldRegion { get; set; }

        /// <summary>
        /// Gets or sets the the address field's postcode
        /// </summary>
        [JsonProperty(PropertyName = POSTCODE)]
        public string Postcode { get; set; }

        /// <summary>
        /// Gets or sets the the address field's po box
        /// </summary>
        [JsonProperty(PropertyName = PO_BOX)]
        public string PoBox { get; set; }



        /// <summary>
        /// No-args constructor
        /// </summary>
        public FieldedAddressRecord() { }
        /// <summary>
        /// Full constructor
        /// </summary>
        /// <param name="house">The house</param>
        /// <param name="houseNumber">The house number</param>
        /// <param name="road">The road</param>
        /// <param name="unit">The unit</param>
        /// <param name="level">The level</param>
        /// <param name="staircase">The staircase</param>
        /// <param name="entrance">The entrance</param>
        /// <param name="suburb">The suburb</param>
        /// <param name="cityDistrict">The city district</param>
        /// <param name="city">The city</param>
        /// <param name="island">The island</param>
        /// <param name="stateDistrict">The state district</param>
        /// <param name="state">The state</param>
        /// <param name="countryRegion">The country region</param>
        /// <param name="country">The country</param>
        /// <param name="worldRegion">The world region</param>
        /// <param name="postcode">The postcode</param>
        /// <param name="poBox">The po box</param>
        public FieldedAddressRecord(
            string house,
            string houseNumber,
            string road,
            string unit,
            string level,
            string staircase,
            string entrance,
            string suburb,
            string cityDistrict,
            string city,
            string island,
            string stateDistrict,
            string state,
            string countryRegion,
            string country,
            string worldRegion,
            string postcode,
            string poBox
            )
        {
            this.House = house;
            this.HouseNumber = houseNumber;
            this.Road = road;
            this.Unit = unit;
            this.Level = level;
            this.Staircase = staircase;
            this.Entrance = entrance;
            this.Suburb = suburb;
            this.CityDistrict = cityDistrict;
            this.City = city;
            this.Island = island;
            this.StateDistrict = stateDistrict;
            this.State = state;
            this.CountryRegion = countryRegion;
            this.Country = country;
            this.WorldRegion = worldRegion;
            this.Postcode = postcode;
            this.PoBox = poBox;
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
                    this.House == other.House,
                    this.HouseNumber == other.HouseNumber,
                    this.Road == other.Road,
                    this.Unit == other.Unit,
                    this.Level == other.Level,
                    this.Staircase == other.Staircase,
                    this.Entrance == other.Entrance,
                    this.Suburb == other.Suburb,
                    this.CityDistrict == other.CityDistrict,
                    this.City == other.City,
                    this.Island == other.Island,
                    this.StateDistrict == other.StateDistrict,
                    this.State == other.State,
                    this.CountryRegion == other.CountryRegion,
                    this.Country == other.Country,
                    this.WorldRegion == other.WorldRegion,
                    this.Postcode == other.Postcode,
                    this.PoBox == other.PoBox
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
            int h0 = this.House != null ? this.House.GetHashCode() : 1;
            int h1 = this.HouseNumber != null ? this.HouseNumber.GetHashCode() : 1;
            int h2 = this.Road != null ? this.Road.GetHashCode() : 1;
            int h3 = this.Unit != null ? this.Unit.GetHashCode() : 1;
            int h4 = this.Level != null ? this.Level.GetHashCode() : 1;
            int h5 = this.Staircase != null ? this.Staircase.GetHashCode() : 1;
            int h6 = this.Entrance != null ? this.Entrance.GetHashCode() : 1;
            int h7 = this.Suburb != null ? this.Suburb.GetHashCode() : 1;
            int h8 = this.CityDistrict != null ? this.CityDistrict.GetHashCode() : 1;
            int h9 = this.City != null ? this.City.GetHashCode() : 1;
            int h10 = this.Island != null ? this.Island.GetHashCode() : 1;
            int h11 = this.StateDistrict != null ? this.StateDistrict.GetHashCode() : 1;
            int h12 = this.State != null ? this.State.GetHashCode() : 1;
            int h13 = this.CountryRegion != null ? this.CountryRegion.GetHashCode() : 1;
            int h14 = this.Country != null ? this.Country.GetHashCode() : 1;
            int h15 = this.WorldRegion != null ? this.WorldRegion.GetHashCode() : 1;
            int h16 = this.Postcode != null ? this.Postcode.GetHashCode() : 1;
            int h17 = this.PoBox != null ? this.PoBox.GetHashCode() : 1;
            return h0 ^ h1 ^ h2 ^ h3 ^ h4 ^ h5 ^ h6 ^ h7 ^ h8 ^ h9 ^ h10 ^ h11 ^ h12 ^ h13 ^ h14 ^ h15 ^ h16 ^ h17;
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

    /// <summary>
    /// Class for representing an unknown field
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class UnknownField : RecordSimilarityField
    {
        public const string DATA = "data";

        /// <summary>
        /// Gets or the unknown field's data
        /// </summary>
        [JsonProperty(PropertyName = DATA)]
        public JToken Data { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The data as a JToken</param>
        public UnknownField(JToken data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Equals override
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj)
        {
            if (obj is UnknownField)
            {
                UnknownField other = obj as UnknownField;
                return JToken.DeepEquals(this.Data, other.Data);
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
            return this.Data != null ? this.Data.GetHashCode() : 1;
        }

        /// <summary>
        /// ToString override.
        /// </summary>
        /// <returns>This unknown field in JSON form</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.Data);
        }

    }

}