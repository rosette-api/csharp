namespace rosette_api
{
    public class Address : IAddress
    {
        public Address(string? house = null, string? houseNumber = null, string? road = null, string? unit = null, string? level = null, string? staircase = null, string? entrance = null, string? suburb = null, string? cityDistrict = null, string? city = null, string? island = null, string? stateDistrict = null, string? state = null, string? countryRegion = null, string? country = null, string? worldRegion = null, string? postCode = null, string? poBox = null)
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
            this.PostCode = postCode;
            this.PoBox = poBox;
        }

        /// <summary>house
        /// <para>
        /// Getter, Setter for the house
        /// </para>
        /// </summary>
        public string? House { get; set; }

        /// <summary>houseNumber
        /// <para>
        /// Getter, Setter for the houseNumber
        /// </para>
        /// </summary>
        public string? HouseNumber { get; set; }

        /// <summary>road
        /// <para>
        /// Getter, Setter for the road
        /// </para>
        /// </summary>
        public string? Road { get; set; }

        /// <summary>unit
        /// <para>
        /// Getter, Setter for the unit
        /// </para>
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>level
        /// <para>
        /// Getter, Setter for the level
        /// </para>
        /// </summary>
        public string? Level { get; set; }

        /// <summary>staircase
        /// <para>
        /// Getter, Setter for the staircase
        /// </para>
        /// </summary>
        public string? Staircase { get; set; }

        /// <summary>entrance
        /// <para>
        /// Getter, Setter for the entrance
        /// </para>
        /// </summary>
        public string? Entrance { get; set; }

        /// <summary>suburb
        /// <para>
        /// Getter, Setter for the suburb
        /// </para>
        /// </summary>
        public string? Suburb { get; set; }

        /// <summary>cityDistrict
        /// <para>
        /// Getter, Setter for the cityDistrict
        /// </para>
        /// </summary>
        public string? CityDistrict { get; set; }

        /// <summary>city
        /// <para>
        /// Getter, Setter for the city
        /// </para>
        /// </summary>
        public string? City { get; set; }

        /// <summary>island
        /// <para>
        /// Getter, Setter for the island
        /// </para>
        /// </summary>
        public string? Island { get; set; }

        /// <summary>stateDistrict
        /// <para>
        /// Getter, Setter for the stateDistrict
        /// </para>
        /// </summary>
        public string? StateDistrict { get; set; }

        /// <summary>state
        /// <para>
        /// Getter, Setter for the state
        /// </para>
        /// </summary>
        public string? State { get; set; }

        /// <summary>countryRegion
        /// <para>
        /// Getter, Setter for the countryRegion
        /// </para>
        /// </summary>
        public string? CountryRegion { get; set; }

        /// <summary>country
        /// <para>
        /// Getter, Setter for the country
        /// </para>
        /// </summary>
        public string? Country { get; set; }

        /// <summary>worldRegion
        /// <para>
        /// Getter, Setter for the worldRegion
        /// </para>
        /// </summary>
        public string? WorldRegion { get; set; }

        /// <summary>postCode
        /// <para>
        /// Getter, Setter for the postCode
        /// </para>
        /// </summary>
        public string? PostCode { get; set; }

        /// <summary>poBox
        /// <para>
        /// Getter, Setter for the poBox
        /// </para>
        /// </summary>
        public string? PoBox { get; set; }

        /// <summary> is this address fielded?
        /// </summary>
        public bool Fielded()
        {
            return true;
        }
    }
}