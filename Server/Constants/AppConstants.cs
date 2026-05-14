namespace Karakatsiya.Constants
{
    public static class AppConstants
    {
        public static class General
        {
            public const string DEFAULT_CURRENCY = "UAH";
            public const int MAX_TITLE_LENGTH = 300;
        }

        public static class Shared
        {
            public const string LOCALHOST = "http://localhost:4200";
            public const string DEV_DOMAIN = "https://karakatsiya.local";
            public const string PWA_MOBILE = "http://192.168.1.50:4200";
            public const string CORS_POLICY_NAME = "AllowAngular";
        }

        public static class Config
        {
            public const string DEFAULT_CONNECTION = "DefaultConnection";
            public const string JWT_KEY = "Jwt:Key";
            public const string JWT_ISSUER = "Jwt:Issuer";
            public const string JWT_AUDIENCE = "Jwt:Audience";
            public const string JWT_EXPIRE_DAYS = "Jwt:ExpireDays";
            public const string SEED_ADMIN_EMAIL = "SeedData:AdminEmail";
            public const string SEED_ADMIN_PASSWORD = "SeedData:AdminPassword";
        }

        public static class MimeTypes
        {
            public const string APPLICATION_JSON = "application/json";
        }

        public static class Security
        {
            public const int OTP_MIN_VALUE = 100000;
            public const int OTP_MAX_VALUE = 999999;
            public const int OTP_EXPIRY_MINUTES = 15;
        }
        /*
        public static class Email
        {
            public const string VERIF_SUBJECT = "Код подтверждения Каракатица";
            public const string VERIF_BODY_TEMPLATE = "Твой код: {0}. У тебя есть {1} минут, потом превратишься в тыкву.";
        }
        */
        public static class SeedData
        {
            public const string ADMIN_CREATED_LOG = "✅ Суперадмин успешно создан!";
        }

        public static class Validation
        {
            public const int MAX_NAME_LENGTH = 200;
            public const int MAX_PHONE_LENGTH = 20;
            public const int MAX_EMAIL_LENGTH = 100;
            public const int MAX_CITY_LENGTH = 100;
            public const int MAX_STREET_LENGTH = 200;
            public const int MAX_HOUSE_NUMBER_LENGTH = 20;
            public const int MAX_TICKET_CODE_LENGTH = 50;
            public const int DECIMAL_PRECISION = 18;
            public const int DECIMAL_SCALE = 2;
            public const int MAX_COMMENT_LENGTH = 1000;
            public const int MAX_URL_LENGTH = 500;
            public const int MAX_SLUG_LENGTH = 150;
        }

        public static class Columns
        {
            public const string CONTACT_PHONE = "Contact_Phone";
            public const string CONTACT_EMAIL = "Contact_Email";
            public const string ADDRESS_CITY = "Address_City";
            public const string ADDRESS_STREET = "Address_Street";
            public const string ADDRESS_HOUSE = "Address_HouseNumber";
        }

        public static class Storage
        {
            public const string WWWROOT_FOLDER = "wwwroot";
            public const string UPLOADS_FOLDER = "uploads";
            public const string EVENTS_IMAGES_SUBFOLDER = "events";
            public const string USER_PHOTOS_SUBFOLDER = "user-content";
            public const long MAX_FILE_SIZE_BYTES = 5 * 1024 * 1024;
            public static readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".webp" };
        }

        public static class Errors
        {
            public const string EVENT_NOT_FOUND = "ERRORS.EVENT_NOT_FOUND";
            public const string INVALID_DATE = "ERRORS.INVALID_START_DATE";
            public const string SERVICE_UNAVAILABLE = "ERRORS.SERVICE_UNAVAILABLE";
            public const string ORGANIZER_NOT_FOUND = "ERRORS.ORGANIZER_NOT_FOUND";
            public const string VERIFICATION_CODE_EXPIRED = "ERRORS.VERIFICATION_CODE_EXPIRED";
            public const string INVALID_VERIFICATION_CODE = "ERRORS.INVALID_VERIFICATION_CODE";
            public const string USER_NOT_FOUND_OR_ALREADY_VERIFIED = "ERRORS.USER_NOT_FOUND_OR_ALREADY_VERIFIED";
            public const string EMAIL_ALREADY_EXISTS = "ERRORS.EMAIL_ALREADY_EXISTS";
            public const string INVALID_CREDENTIALS = "ERRORS.INVALID_CREDENTIALS";
            public const string VALIDATION_FAILED = "ERRORS.VALIDATION_FAILED";
            public const string INTERNAL_SERVER_ERROR = "ERRORS.INTERNAL_SERVER_ERROR";
            public const string CONFIG_MISSING_JWT = "JWT Key is missing in configuration!";
            public const string MIDDLEWARE_FATAL_LOG = "Ахтунг! В контроллере пиздец: {Message}";
            public const string USER_NOT_FOUND = "ERRORS.USER_NOT_FOUND";
            public const string ALREADY_APPLIED_OR_ADMIN = "ERRORS.ALREADY_APPLIED_OR_ADMIN";
            public const string NO_CONTACTS_PROVIDED = "ERRORS.NO_CONTACTS_PROVIDED";
            public const string INVALID_EMAIL = "ERRORS.INVALID_EMAIL";
            public const string INVALID_TOKEN = "ERRORS.INVALID_TOKEN";
            public const string NOT_PENDING_ORGANIZER = "ERRORS.NOT_PENDING_ORGANIZER";
        }

        public static class Success
        {
            public const string EVENT_CREATED = "SUCCESS.EVENT_CREATED";
            public const string REQUEST_APPROVED = "SUCCESS.REQUEST_APPROVED";
            public const string VERIFICATION_CODE_SENT = "SUCCESS.VERIFICATION_CODE_SENT";
            public const string APPLICATION_SUBMITTED = "SUCCESS.APPLICATION_SUBMITTED";
            public const string ORGANIZER_APPROVED = "SUCCESS.ORGANIZER_APPROVED";
            public const string ORGANIZER_REJECTED = "SUCCESS.ORGANIZER_REJECTED";
        }
    }
}