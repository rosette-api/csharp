using System;

namespace rosette_api {
    /// <summary>Name Class
    /// <para>
    /// Name: Custom Datatype to use in Matched Name endpoint
    /// </para>
    /// </summary>
    public class Name
    {
        /// <summary>Name
        /// <para>
        /// Name: Custom Datatype to use in Matched Name endpoint
        /// </para>
        /// </summary>
        /// <param name="Text">(string, optional): Text describing the name</param>
        /// <param name="Language">(string, optional): Language: ISO 639-3 code (ignored for the /language endpoint)</param>
        /// <param name="Script">(string, optional): ISO 15924 code for the name's script</param>
        /// <param name="EntityType">(string, optional): Entity type of the name: PERSON, LOCATION, or ORGANIZATION</param>
        /// <param name="Gender">(Gender, optional): gender: Gender of the name: Female, Male, NonBinary</param>
        public Name(string Text = null, string Language = null, string Script = null, string EntityType = null, Gender? Gender = null)
        {
            text = Text;
            language = Language;
            script = Script;
            entityType = EntityType;
            gender = Gender;
        }

        /// <summary>text
        /// <para>
        /// Getter, Setter for the text
        /// text: Text describing the name
        /// </para>
        /// </summary>
        public string text { get; set; }

        /// <summary>language
        /// <para>
        /// Getter, Setter for the language
        /// language: Language: ISO 639-3 code
        /// </para>
        /// </summary>
        public string language { get; set; }

        /// <summary>script
        /// <para>
        /// Getter, Setter for the script
        /// script: ISO 15924 code for the name's script
        /// </para>
        /// </summary>
        public string script { get; set; }

        /// <summary>entityType
        /// <para>
        /// Getter, Setter for the entityType
        /// entityType: Entity type of the name: PERSON, LOCATION, or ORGANIZATION
        /// </para>
        /// </summary>
        public string entityType { get; set; }

        /// <summary>gender
        /// <para>
        /// Getter, Setter for the gender
        /// gender: Gender of the name
        /// </para>
        /// </summary>
        public Gender? gender { get; set; }

    }
}
