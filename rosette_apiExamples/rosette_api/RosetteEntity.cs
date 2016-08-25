using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rosette_api
{
    /// <summary>
    /// A class for representing an identified entity and its properties
    /// </summary>
    public class RosetteEntity
    {
        /// <summary>
        /// The Location entity type
        /// </summary>
        public const string LOCATION = "LOCATION";
        /// <summary>
        /// The organization entity type
        /// </summary>
        public const string ORGANIZATION = "ORGANIZATION";
        /// <summary>
        /// The person entity type
        /// </summary>
        public const string PERSON = "PERSON";
        /// <summary>
        /// The product entity type
        /// </summary>
        public const string PRODUCT = "PRODUCT";
        /// <summary>
        /// The title entity type
        /// </summary>
        public const string TITLE = "TITLE";
        /// <summary>
        /// The nationality entity type
        /// </summary>
        public const string NATIONALITY = "NATIONALITY";
        /// <summary>
        /// The religion entity type
        /// </summary>
        public const string RELIGION = "RELIGION";
        /// <summary>
        /// The credit card number entity type
        /// </summary>
        public const string CREDIT_CARD_NUM = "IDENTIFIER:CREDIT_CARD_NUM";
        /// <summary>
        /// The email address entity type
        /// </summary>
        public const string EMAIL = "IDENTIFIER:EMAIL";
        /// <summary>
        /// The money entity type
        /// </summary>
        public const string MONEY = "IDENTIFIER:MONEY";
        /// <summary>
        /// The personal ID number entity type
        /// </summary>
        public const string PERSONAL_ID_NUM = "IDENTIFIER:PERSONAL_ID_NUM";
        /// <summary>
        /// The phone number entity type
        /// </summary>
        public const string PHONE_NUMBER = "IDENTIFIER:PHONE_NUMBER";
        /// <summary>
        /// The URL entity type
        /// </summary>
        public const string URL = "IDENTIFIER:URL";
        /// <summary>
        /// The date entity type
        /// </summary>
        public const string DATE = "TEMPORAL:DATE";
        /// <summary>
        /// The time entity type
        /// </summary>
        public const string TIME = "TEMPORAL:TIME";
        /// <summary>
        /// The distance entity type
        /// </summary>
        public const string DISTANCE = "IDENTIFIER:DISTANCE";
        /// <summary>
        /// The latitude-longitude entity type
        /// </summary>
        public const string LATITUDE_LONGITUDE = "IDENTIFIER:LATITUDE_LONGITUDE";

        /// <summary>
        /// The entity's name
        /// </summary>
        public String Mention;

        /// <summary>
        /// The entity's ID
        /// </summary>
        public EntityID ID;

        /// <summary>
        /// The entity's type
        /// </summary>
        public String EntityType;

        /// <summary>
        /// Creates an entity
        /// </summary>
        /// <param name="mention">The mention of the entity</param>
        /// <param name="id">The mention's id</param>
        /// <param name="entityType">The entity type</param>
        public RosetteEntity(String mention, EntityID id, String entityType)
        {
            this.Mention = mention;
            this.ID = id;
            this.EntityType = entityType;
        }
    }
}
