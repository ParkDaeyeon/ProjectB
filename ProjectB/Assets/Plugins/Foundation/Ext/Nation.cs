using System;
using System.Collections.Generic;
namespace Ext
{
    public static class Nation_ISO3166
    {
        public static Dictionary<string, string> Map { private set; get; }

        public static List<string> IsoCodeOrderList { private set; get; }

        public static string GetName(string isoCode)
        {
            if (!Nation_ISO3166.IsOpened)
                Nation_ISO3166.Open();

            string value;
            if (Nation_ISO3166.Map.TryGetValue(isoCode.ToUpper(), out value))
                return value;

            return "";
        }

        public static int GetOrder(string isoCode)
        {
            return Nation_ISO3166.IsoCodeOrderList.IndexOf(isoCode);
        }

        public static bool IsOpened { get { return null != Nation_ISO3166.Map; } }
        public static void Open()
        {
            if (Nation_ISO3166.IsOpened)
                Nation_ISO3166.Close();

            Nation_ISO3166.Map = new Dictionary<string, string>(256);

            // NOTE: ISO 3166 fullver

            //AD    Andorra
            //AE    United Arab Emirates
            //AF    Afghanistan
            //AG    Antigua and Barbuda
            //AI    Anguilla
            //AL    Albania
            //AM    Armenia
            //AO    Angola
            //AQ    Antarctica
            //AR    Argentina
            //AS    American Samoa
            //AT    Austria
            //AU    Australia
            //AW    Aruba
            //AX    Åland Islands
            //AZ    Azerbaijan
            //BA    Bosnia and Herzegovina
            //BB    Barbados
            //BD    Bangladesh
            //BE    Belgium
            //BF    Burkina Faso
            //BG    Bulgaria
            //BH    Bahrain
            //BI    Burundi
            //BJ    Benin
            //BL    Saint Barthélemy
            //BM    Bermuda
            //BN    Brunei Darussalam
            //BO    Bolivia, Plurinational State of
            //BQ    Bonaire, Sint Eustatius and Saba
            //BR    Brazil
            //BS    Bahamas
            //BT    Bhutan
            //BV    Bouvet Island
            //BW    Botswana
            //BY    Belarus
            //BZ    Belize
            //CA    Canada
            //CC    Cocos(Keeling) Islands
            //CD    Congo, the Democratic Republic of the
            //CF    Central African Republic
            //CG    Congo
            //CH    Switzerland
            //CI    Côte d'Ivoire
            //CK    Cook Islands
            //CL    Chile
            //CM    Cameroon
            //CN    China
            //CO    Colombia
            //CR    Costa Rica
            //CU    Cuba
            //CV    Cape Verde
            //CW    Curaçao
            //CX    Christmas Island
            //CY    Cyprus
            //CZ    Czech Republic
            //DE    Germany
            //DJ    Djibouti
            //DK    Denmark
            //DM    Dominica
            //DO    Dominican Republic
            //DZ    Algeria
            //EC    Ecuador
            //EE    Estonia
            //EG    Egypt
            //EH    Western Sahara
            //ER    Eritrea
            //ES    Spain
            //ET    Ethiopia
            //FI    Finland
            //FJ    Fiji
            //FK    Falkland Islands (Malvinas)
            //FM    Micronesia, Federated States of
            //FO    Faroe Islands
            //FR    France
            //GA    Gabon
            //GB    United Kingdom
            //GD    Grenada
            //GE    Georgia
            //GF    French Guiana
            //GG    Guernsey
            //GH    Ghana
            //GI    Gibraltar
            //GL    Greenland
            //GM    Gambia
            //GN    Guinea
            //GP    Guadeloupe
            //GQ    Equatorial Guinea
            //GR    Greece
            //GS    South Georgia and the South Sandwich Islands
            //GT    Guatemala
            //GU    Guam
            //GW    Guinea - Bissau
            //GY    Guyana
            //HK    Hong Kong
            //HM    Heard Island and McDonald Islands
            //HN    Honduras
            //HR    Croatia
            //HT    Haiti
            //HU    Hungary
            //ID    Indonesia
            //IE    Ireland
            //IL    Israel
            //IM    Isle of Man
            //IN    India
            //IO    British Indian Ocean Territory
            //IQ    Iraq
            //IR    Iran, Islamic Republic of
            //IS    Iceland
            //IT    Italy
            //JE    Jersey
            //JM    Jamaica
            //JO    Jordan
            //JP    Japan
            //KE    Kenya
            //KG    Kyrgyzstan
            //KH    Cambodia
            //KI    Kiribati
            //KM    Comoros
            //KN    Saint Kitts and Nevis
            //KP    Korea, Democratic People's Republic of
            //KR    Korea, Republic of
            //KW    Kuwait
            //KY    Cayman Islands
            //KZ    Kazakhstan
            //LA    Lao People's Democratic Republic
            //LB    Lebanon
            //LC    Saint Lucia
            //LI    Liechtenstein
            //LK    Sri Lanka
            //LR    Liberia
            //LS    Lesotho
            //LT    Lithuania
            //LU    Luxembourg
            //LV    Latvia
            //LY    Libya
            //MA    Morocco
            //MC    Monaco
            //MD    Moldova, Republic of
            //ME    Montenegro
            //MF    Saint Martin(French part)
            //MG    Madagascar
            //MH    Marshall Islands
            //MK    Macedonia, the Former Yugoslav Republic of
            //ML    Mali
            //MM    Myanmar
            //MN    Mongolia
            //MO    Macao
            //MP    Northern Mariana Islands
            //MQ    Martinique
            //MR    Mauritania
            //MS    Montserrat
            //MT    Malta
            //MU    Mauritius
            //MV    Maldives
            //MW    Malawi
            //MX    Mexico
            //MY    Malaysia
            //MZ    Mozambique
            //NA    Namibia
            //NC    New Caledonia
            //NE    Niger
            //NF    Norfolk Island
            //NG    Nigeria
            //NI    Nicaragua
            //NL    Netherlands
            //NO    Norway
            //NP    Nepal
            //NR    Nauru
            //NU    Niue
            //NZ    New Zealand
            //OM    Oman
            //PA    Panama
            //PE    Peru
            //PF    French Polynesia
            //PG    Papua New Guinea
            //PH    Philippines
            //PK    Pakistan
            //PL    Poland
            //PM    Saint Pierre and Miquelon
            //PN    Pitcairn
            //PR    Puerto Rico
            //PS    Palestine, State of
            //PT    Portugal
            //PW    Palau
            //PY    Paraguay
            //QA    Qatar
            //RE    Réunion
            //RO    Romania
            //RS    Serbia
            //RU    Russian Federation
            //RW    Rwanda
            //SA    Saudi Arabia
            //SB    Solomon Islands
            //SC    Seychelles
            //SD    Sudan
            //SE    Sweden
            //SG    Singapore
            //SH    Saint Helena, Ascension and Tristan da Cunha
            //SI    Slovenia
            //SJ    Svalbard and Jan Mayen
            //SK    Slovakia
            //SL    Sierra Leone
            //SM    San Marino
            //SN    Senegal
            //SO    Somalia
            //SR    Suriname
            //SS    South Sudan
            //ST    Sao Tome and Principe
            //SV    El Salvador
            //SX    Sint Maarten(Dutch part)
            //SY    Syrian Arab Republic
            //SZ    Swaziland
            //TC    Turks and Caicos Islands
            //TD    Chad
            //TF    French Southern Territories
            //TG    Togo
            //TH    Thailand
            //TJ    Tajikistan
            //TK    Tokelau
            //TL    Timor - Leste
            //TM    Turkmenistan
            //TN    Tunisia
            //TO    Tonga
            //TR    Turkey
            //TT    Trinidad and Tobago
            //TV    Tuvalu
            //TW    Taiwan, Province of China
            //TZ    Tanzania, United Republic of
            //UA    Ukraine
            //UG    Uganda
            //UM    United States Minor Outlying Islands
            //US    United States
            //UY    Uruguay
            //UZ    Uzbekistan
            //VA    Holy See(Vatican City State)
            //VC    Saint Vincent and the Grenadines
            //VE    Venezuela, Bolivarian Republic of
            //VG    Virgin Islands, British
            //VI    Virgin Islands, U.S.
            //VN    Viet Nam
            //VU    Vanuatu
            //WF    Wallis and Futuna
            //WS    Samoa
            //YE    Yemen
            //YT    Mayotte
            //ZA    South Africa
            //ZM    Zambia
            //ZW    Zimbabwe

            Nation_ISO3166.Map.Add("AD", "Andorra");
            Nation_ISO3166.Map.Add("AE", "United Arab Emirates");
            Nation_ISO3166.Map.Add("AF", "Afghanistan");
            Nation_ISO3166.Map.Add("AG", "Antigua and Barbuda");
            Nation_ISO3166.Map.Add("AI", "Anguilla");
            Nation_ISO3166.Map.Add("AL", "Albania");
            Nation_ISO3166.Map.Add("AM", "Armenia");
            Nation_ISO3166.Map.Add("AO", "Angola");
            Nation_ISO3166.Map.Add("AQ", "Antarctica");
            Nation_ISO3166.Map.Add("AR", "Argentina");
            Nation_ISO3166.Map.Add("AS", "American Samoa");
            Nation_ISO3166.Map.Add("AT", "Austria");
            Nation_ISO3166.Map.Add("AU", "Australia");
            Nation_ISO3166.Map.Add("AW", "Aruba");
            Nation_ISO3166.Map.Add("AX", "Åland Islands");
            Nation_ISO3166.Map.Add("AZ", "Azerbaijan");
            Nation_ISO3166.Map.Add("BA", "Bosnia and Herzegovina");
            Nation_ISO3166.Map.Add("BB", "Barbados");
            Nation_ISO3166.Map.Add("BD", "Bangladesh");
            Nation_ISO3166.Map.Add("BE", "Belgium");
            Nation_ISO3166.Map.Add("BF", "Burkina Faso");
            Nation_ISO3166.Map.Add("BG", "Bulgaria");
            Nation_ISO3166.Map.Add("BH", "Bahrain");
            Nation_ISO3166.Map.Add("BI", "Burundi");
            Nation_ISO3166.Map.Add("BJ", "Benin");
            Nation_ISO3166.Map.Add("BL", "Saint Barthélemy");
            Nation_ISO3166.Map.Add("BM", "Bermuda");
            Nation_ISO3166.Map.Add("BN", "Brunei Darussalam");
            Nation_ISO3166.Map.Add("BO", "Bolivia, Plurinational State of");
            Nation_ISO3166.Map.Add("BQ", "Bonaire, Sint Eustatius and Saba");
            Nation_ISO3166.Map.Add("BR", "Brazil");
            Nation_ISO3166.Map.Add("BS", "Bahamas");
            Nation_ISO3166.Map.Add("BT", "Bhutan");
            Nation_ISO3166.Map.Add("BV", "Bouvet Island");
            Nation_ISO3166.Map.Add("BW", "Botswana");
            Nation_ISO3166.Map.Add("BY", "Belarus");
            Nation_ISO3166.Map.Add("BZ", "Belize");
            Nation_ISO3166.Map.Add("CA", "Canada");
            Nation_ISO3166.Map.Add("CC", "Cocos(Keeling) Islands");
            Nation_ISO3166.Map.Add("CD", "Congo, the Democratic Republic of the");
            Nation_ISO3166.Map.Add("CF", "Central African Republic");
            Nation_ISO3166.Map.Add("CG", "Congo");
            Nation_ISO3166.Map.Add("CH", "Switzerland");
            Nation_ISO3166.Map.Add("CI", "Côte d'Ivoire");
            Nation_ISO3166.Map.Add("CK", "Cook Islands");
            Nation_ISO3166.Map.Add("CL", "Chile");
            Nation_ISO3166.Map.Add("CM", "Cameroon");
            Nation_ISO3166.Map.Add("CN", "China");
            Nation_ISO3166.Map.Add("CO", "Colombia");
            Nation_ISO3166.Map.Add("CR", "Costa Rica");
            Nation_ISO3166.Map.Add("CU", "Cuba");
            Nation_ISO3166.Map.Add("CV", "Cape Verde");
            Nation_ISO3166.Map.Add("CW", "Curaçao");
            Nation_ISO3166.Map.Add("CX", "Christmas Island");
            Nation_ISO3166.Map.Add("CY", "Cyprus");
            Nation_ISO3166.Map.Add("CZ", "Czech Republic");
            Nation_ISO3166.Map.Add("DE", "Germany");
            Nation_ISO3166.Map.Add("DJ", "Djibouti");
            Nation_ISO3166.Map.Add("DK", "Denmark");
            Nation_ISO3166.Map.Add("DM", "Dominica");
            Nation_ISO3166.Map.Add("DO", "Dominican Republic");
            Nation_ISO3166.Map.Add("DZ", "Algeria");
            Nation_ISO3166.Map.Add("EC", "Ecuador");
            Nation_ISO3166.Map.Add("EE", "Estonia");
            Nation_ISO3166.Map.Add("EG", "Egypt");
            Nation_ISO3166.Map.Add("EH", "Western Sahara");
            Nation_ISO3166.Map.Add("ER", "Eritrea");
            Nation_ISO3166.Map.Add("ES", "Spain");
            Nation_ISO3166.Map.Add("ET", "Ethiopia");
            Nation_ISO3166.Map.Add("FI", "Finland");
            Nation_ISO3166.Map.Add("FJ", "Fiji");
            Nation_ISO3166.Map.Add("FK", "Falkland Islands (Malvinas)");
            Nation_ISO3166.Map.Add("FM", "Micronesia, Federated States of");
            Nation_ISO3166.Map.Add("FO", "Faroe Islands");
            Nation_ISO3166.Map.Add("FR", "France");
            Nation_ISO3166.Map.Add("GA", "Gabon");
            Nation_ISO3166.Map.Add("GB", "United Kingdom");
            Nation_ISO3166.Map.Add("GD", "Grenada");
            Nation_ISO3166.Map.Add("GE", "Georgia");
            Nation_ISO3166.Map.Add("GF", "French Guiana");
            Nation_ISO3166.Map.Add("GG", "Guernsey");
            Nation_ISO3166.Map.Add("GH", "Ghana");
            Nation_ISO3166.Map.Add("GI", "Gibraltar");
            Nation_ISO3166.Map.Add("GL", "Greenland");
            Nation_ISO3166.Map.Add("GM", "Gambia");
            Nation_ISO3166.Map.Add("GN", "Guinea");
            Nation_ISO3166.Map.Add("GP", "Guadeloupe");
            Nation_ISO3166.Map.Add("GQ", "Equatorial Guinea");
            Nation_ISO3166.Map.Add("GR", "Greece");
            Nation_ISO3166.Map.Add("GS", "South Georgia and the South Sandwich Islands");
            Nation_ISO3166.Map.Add("GT", "Guatemala");
            Nation_ISO3166.Map.Add("GU", "Guam");
            Nation_ISO3166.Map.Add("GW", "Guinea - Bissau");
            Nation_ISO3166.Map.Add("GY", "Guyana");
            Nation_ISO3166.Map.Add("HK", "Hong Kong");
            Nation_ISO3166.Map.Add("HM", "Heard Island and McDonald Islands");
            Nation_ISO3166.Map.Add("HN", "Honduras");
            Nation_ISO3166.Map.Add("HR", "Croatia");
            Nation_ISO3166.Map.Add("HT", "Haiti");
            Nation_ISO3166.Map.Add("HU", "Hungary");
            Nation_ISO3166.Map.Add("ID", "Indonesia");
            Nation_ISO3166.Map.Add("IE", "Ireland");
            Nation_ISO3166.Map.Add("IL", "Israel");
            Nation_ISO3166.Map.Add("IM", "Isle of Man");
            Nation_ISO3166.Map.Add("IN", "India");
            Nation_ISO3166.Map.Add("IO", "British Indian Ocean Territory");
            Nation_ISO3166.Map.Add("IQ", "Iraq");
            Nation_ISO3166.Map.Add("IR", "Iran, Islamic Republic of");
            Nation_ISO3166.Map.Add("IS", "Iceland");
            Nation_ISO3166.Map.Add("IT", "Italy");
            Nation_ISO3166.Map.Add("JE", "Jersey");
            Nation_ISO3166.Map.Add("JM", "Jamaica");
            Nation_ISO3166.Map.Add("JO", "Jordan");
            Nation_ISO3166.Map.Add("JP", "Japan");
            Nation_ISO3166.Map.Add("KE", "Kenya");
            Nation_ISO3166.Map.Add("KG", "Kyrgyzstan");
            Nation_ISO3166.Map.Add("KH", "Cambodia");
            Nation_ISO3166.Map.Add("KI", "Kiribati");
            Nation_ISO3166.Map.Add("KM", "Comoros");
            Nation_ISO3166.Map.Add("KN", "Saint Kitts and Nevis");
            Nation_ISO3166.Map.Add("KP", "Korea, Democratic People's Republic of");
            Nation_ISO3166.Map.Add("KR", "Korea, Republic of");
            Nation_ISO3166.Map.Add("KW", "Kuwait");
            Nation_ISO3166.Map.Add("KY", "Cayman Islands");
            Nation_ISO3166.Map.Add("KZ", "Kazakhstan");
            Nation_ISO3166.Map.Add("LA", "Lao People's Democratic Republic");
            Nation_ISO3166.Map.Add("LB", "Lebanon");
            Nation_ISO3166.Map.Add("LC", "Saint Lucia");
            Nation_ISO3166.Map.Add("LI", "Liechtenstein");
            Nation_ISO3166.Map.Add("LK", "Sri Lanka");
            Nation_ISO3166.Map.Add("LR", "Liberia");
            Nation_ISO3166.Map.Add("LS", "Lesotho");
            Nation_ISO3166.Map.Add("LT", "Lithuania");
            Nation_ISO3166.Map.Add("LU", "Luxembourg");
            Nation_ISO3166.Map.Add("LV", "Latvia");
            Nation_ISO3166.Map.Add("LY", "Libya");
            Nation_ISO3166.Map.Add("MA", "Morocco");
            Nation_ISO3166.Map.Add("MC", "Monaco");
            Nation_ISO3166.Map.Add("MD", "Moldova, Republic of");
            Nation_ISO3166.Map.Add("ME", "Montenegro");
            Nation_ISO3166.Map.Add("MF", "Saint Martin(French part)");
            Nation_ISO3166.Map.Add("MG", "Madagascar");
            Nation_ISO3166.Map.Add("MH", "Marshall Islands");
            Nation_ISO3166.Map.Add("MK", "Macedonia, the Former Yugoslav Republic of");
            Nation_ISO3166.Map.Add("ML", "Mali");
            Nation_ISO3166.Map.Add("MM", "Myanmar");
            Nation_ISO3166.Map.Add("MN", "Mongolia");
            Nation_ISO3166.Map.Add("MO", "Macao");
            Nation_ISO3166.Map.Add("MP", "Northern Mariana Islands");
            Nation_ISO3166.Map.Add("MQ", "Martinique");
            Nation_ISO3166.Map.Add("MR", "Mauritania");
            Nation_ISO3166.Map.Add("MS", "Montserrat");
            Nation_ISO3166.Map.Add("MT", "Malta");
            Nation_ISO3166.Map.Add("MU", "Mauritius");
            Nation_ISO3166.Map.Add("MV", "Maldives");
            Nation_ISO3166.Map.Add("MW", "Malawi");
            Nation_ISO3166.Map.Add("MX", "Mexico");
            Nation_ISO3166.Map.Add("MY", "Malaysia");
            Nation_ISO3166.Map.Add("MZ", "Mozambique");
            Nation_ISO3166.Map.Add("NA", "Namibia");
            Nation_ISO3166.Map.Add("NC", "New Caledonia");
            Nation_ISO3166.Map.Add("NE", "Niger");
            Nation_ISO3166.Map.Add("NF", "Norfolk Island");
            Nation_ISO3166.Map.Add("NG", "Nigeria");
            Nation_ISO3166.Map.Add("NI", "Nicaragua");
            Nation_ISO3166.Map.Add("NL", "Netherlands");
            Nation_ISO3166.Map.Add("NO", "Norway");
            Nation_ISO3166.Map.Add("NP", "Nepal");
            Nation_ISO3166.Map.Add("NR", "Nauru");
            Nation_ISO3166.Map.Add("NU", "Niue");
            Nation_ISO3166.Map.Add("NZ", "New Zealand");
            Nation_ISO3166.Map.Add("OM", "Oman");
            Nation_ISO3166.Map.Add("PA", "Panama");
            Nation_ISO3166.Map.Add("PE", "Peru");
            Nation_ISO3166.Map.Add("PF", "French Polynesia");
            Nation_ISO3166.Map.Add("PG", "Papua New Guinea");
            Nation_ISO3166.Map.Add("PH", "Philippines");
            Nation_ISO3166.Map.Add("PK", "Pakistan");
            Nation_ISO3166.Map.Add("PL", "Poland");
            Nation_ISO3166.Map.Add("PM", "Saint Pierre and Miquelon");
            Nation_ISO3166.Map.Add("PN", "Pitcairn");
            Nation_ISO3166.Map.Add("PR", "Puerto Rico");
            Nation_ISO3166.Map.Add("PS", "Palestine, State of");
            Nation_ISO3166.Map.Add("PT", "Portugal");
            Nation_ISO3166.Map.Add("PW", "Palau");
            Nation_ISO3166.Map.Add("PY", "Paraguay");
            Nation_ISO3166.Map.Add("QA", "Qatar");
            Nation_ISO3166.Map.Add("RE", "Réunion");
            Nation_ISO3166.Map.Add("RO", "Romania");
            Nation_ISO3166.Map.Add("RS", "Serbia");
            Nation_ISO3166.Map.Add("RU", "Russian Federation");
            Nation_ISO3166.Map.Add("RW", "Rwanda");
            Nation_ISO3166.Map.Add("SA", "Saudi Arabia");
            Nation_ISO3166.Map.Add("SB", "Solomon Islands");
            Nation_ISO3166.Map.Add("SC", "Seychelles");
            Nation_ISO3166.Map.Add("SD", "Sudan");
            Nation_ISO3166.Map.Add("SE", "Sweden");
            Nation_ISO3166.Map.Add("SG", "Singapore");
            Nation_ISO3166.Map.Add("SH", "Saint Helena, Ascension and Tristan da Cunha");
            Nation_ISO3166.Map.Add("SI", "Slovenia");
            Nation_ISO3166.Map.Add("SJ", "Svalbard and Jan Mayen");
            Nation_ISO3166.Map.Add("SK", "Slovakia");
            Nation_ISO3166.Map.Add("SL", "Sierra Leone");
            Nation_ISO3166.Map.Add("SM", "San Marino");
            Nation_ISO3166.Map.Add("SN", "Senegal");
            Nation_ISO3166.Map.Add("SO", "Somalia");
            Nation_ISO3166.Map.Add("SR", "Suriname");
            Nation_ISO3166.Map.Add("SS", "South Sudan");
            Nation_ISO3166.Map.Add("ST", "Sao Tome and Principe");
            Nation_ISO3166.Map.Add("SV", "El Salvador");
            Nation_ISO3166.Map.Add("SX", "Sint Maarten(Dutch part)");
            Nation_ISO3166.Map.Add("SY", "Syrian Arab Republic");
            Nation_ISO3166.Map.Add("SZ", "Swaziland");
            Nation_ISO3166.Map.Add("TC", "Turks and Caicos Islands");
            Nation_ISO3166.Map.Add("TD", "Chad");
            Nation_ISO3166.Map.Add("TF", "French Southern Territories");
            Nation_ISO3166.Map.Add("TG", "Togo");
            Nation_ISO3166.Map.Add("TH", "Thailand");
            Nation_ISO3166.Map.Add("TJ", "Tajikistan");
            Nation_ISO3166.Map.Add("TK", "Tokelau");
            Nation_ISO3166.Map.Add("TL", "Timor - Leste");
            Nation_ISO3166.Map.Add("TM", "Turkmenistan");
            Nation_ISO3166.Map.Add("TN", "Tunisia");
            Nation_ISO3166.Map.Add("TO", "Tonga");
            Nation_ISO3166.Map.Add("TR", "Turkey");
            Nation_ISO3166.Map.Add("TT", "Trinidad and Tobago");
            Nation_ISO3166.Map.Add("TV", "Tuvalu");
            Nation_ISO3166.Map.Add("TW", "Taiwan, Province of China");
            Nation_ISO3166.Map.Add("TZ", "Tanzania, United Republic of");
            Nation_ISO3166.Map.Add("UA", "Ukraine");
            Nation_ISO3166.Map.Add("UG", "Uganda");
            Nation_ISO3166.Map.Add("UM", "United States Minor Outlying Islands");
            Nation_ISO3166.Map.Add("US", "United States");
            Nation_ISO3166.Map.Add("UY", "Uruguay");
            Nation_ISO3166.Map.Add("UZ", "Uzbekistan");
            Nation_ISO3166.Map.Add("VA", "Holy See(Vatican City State)");
            Nation_ISO3166.Map.Add("VC", "Saint Vincent and the Grenadines");
            Nation_ISO3166.Map.Add("VE", "Venezuela, Bolivarian Republic of");
            Nation_ISO3166.Map.Add("VG", "Virgin Islands, British");
            Nation_ISO3166.Map.Add("VI", "Virgin Islands, U.S.");
            Nation_ISO3166.Map.Add("VN", "Viet Nam");
            Nation_ISO3166.Map.Add("VU", "Vanuatu");
            Nation_ISO3166.Map.Add("WF", "Wallis and Futuna");
            Nation_ISO3166.Map.Add("WS", "Samoa");
            Nation_ISO3166.Map.Add("YE", "Yemen");
            Nation_ISO3166.Map.Add("YT", "Mayotte");
            Nation_ISO3166.Map.Add("ZA", "South Africa");
            Nation_ISO3166.Map.Add("ZM", "Zambia");
            Nation_ISO3166.Map.Add("ZW", "Zimbabwe");

            Nation_ISO3166.IsoCodeOrderList = new List<string>(Nation_ISO3166.Map.Count);
            foreach (string isoCode in Nation_ISO3166.Map.Keys)
                Nation_ISO3166.IsoCodeOrderList.Add(isoCode);
        }

        public static void Close()
        {
            Nation_ISO3166.Map = null;
            Nation_ISO3166.IsoCodeOrderList = null;
        }
    }



    public static class Nation
    {
        public static Dictionary<string, string> Map { get { return Nation_ISO3166.Map; } }

        public static List<string> IsoCodeOrderList { get { return Nation_ISO3166.IsoCodeOrderList; } }

        public class UnityLanguage
        {
            public string WebIsoCode;
            public string LanguageStr;
            public UnityLanguage(string webIsoCode, string languageStr)
            {
                this.WebIsoCode = webIsoCode;
                this.LanguageStr = languageStr;
            }
        }
        public static Dictionary<string, UnityLanguage> UnityLanguages { private set; get; }
        public static UnityLanguage GetLanguage(string isoCode)
        {
            if (!Nation.IsOpened)
                Nation.Open();

            if (string.IsNullOrEmpty(isoCode))
                return null;

            UnityLanguage value = null;
            if (Nation.UnityLanguages.TryGetValue(isoCode, out value))
                return value;

            return null;
        }
        public static string GetLanguageStr(string isoCode)
        {
            var lan = Nation.GetLanguage(isoCode);
            if (null == lan)
                return null;

            return lan.LanguageStr;
        }
        public static IEnumerable<UnityLanguage> GetLanguages()
        {
            var enumerator = Nation.UnityLanguages.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var e = enumerator.Current;
                yield return e.Value;
            }
        }





        public static string GetName(string isoCode)
        {
            string value = Nation_ISO3166.GetName(isoCode);
            if (string.IsNullOrEmpty(value))
                value = "Unite Nations";

            return value;
        }
        public static int GetOrder(string isoCode)
        {
            int value = Nation_ISO3166.GetOrder(isoCode);
            if (-1 == value)
                value = Nation_ISO3166.IsoCodeOrderList.Count - 1;

            return value;
        }

        public static bool IsOpened { get { return null != Nation_ISO3166.Map; } }

        const string CLASSNAME = "Nation";
        public static void Open()
        {
            if (Nation.IsOpened)
                Nation.Close();

            Nation_ISO3166.Open();
            Nation_ISO3166.IsoCodeOrderList.Add("UN");



            UnityLanguage[] unityLanguageList = {
            new UnityLanguage("at",     "at")   ,
            new UnityLanguage("au",     "au")   ,
            new UnityLanguage("be",     "be")   ,
            new UnityLanguage("cz",     "cz")   ,
            new UnityLanguage("dk",     "dk")   ,
            new UnityLanguage("fi",     "fi")   ,
            new UnityLanguage("gr",     "gr")   ,
            new UnityLanguage("ie",     "it")   ,
            new UnityLanguage("mx",     "mx")   ,
            new UnityLanguage("nl",     "nl")   ,
            new UnityLanguage("no",     "no")   ,
            new UnityLanguage("nz",     "nz")   ,
            new UnityLanguage("pt",     "pt")   ,
            new UnityLanguage("se",     "se")   ,
            new UnityLanguage("gb",     "uk")   ,
            new UnityLanguage("za",     "za")   ,
            new UnityLanguage("br",     "br")   ,
            new UnityLanguage("ca",     "ca")   ,
            new UnityLanguage("ch",     "ch")   ,
            new UnityLanguage("cn",     "cn")   ,
            new UnityLanguage("de",     "de")   ,
            new UnityLanguage("es",     "es")   ,
            new UnityLanguage("fr",     "fr")   ,
            new UnityLanguage("hk",     "hkg")  ,
            new UnityLanguage("hu",     "hu")   ,
            new UnityLanguage("it",     "it")   ,
            new UnityLanguage("jp",     "jp")   ,
            new UnityLanguage("kr",     "kr")   ,
            new UnityLanguage("us",     "us")   ,
            };

            Nation.UnityLanguages = new Dictionary<string, UnityLanguage>(unityLanguageList.Length);
            for (int n = 0; n < unityLanguageList.Length; ++n)
                if (!Nation.UnityLanguages.ContainsKey(unityLanguageList[n].WebIsoCode))
                    Nation.UnityLanguages.Add(unityLanguageList[n].WebIsoCode, unityLanguageList[n]);
        }


        public static void Close()
        {
            Nation_ISO3166.Close();

            Nation.UnityLanguages = null;
        }
    }
}