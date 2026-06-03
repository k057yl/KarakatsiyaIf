namespace Karakatsiya.Constants
{
    public static class AppConstants
    {
        public static class General
        {
            public const string DEFAULT_CURRENCY = "UAH";
            public const int MAX_TITLE_LENGTH = 300;
            public const string NOT_NAME = "GENERAL.NOT_NAME";
            public const string SLUG_DEFAULT_PERFORMER = "performer";
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
            public const string TG_BOT_TOKEN = "TelegramBotSettings:BotToken";
            public const string GEO_USER_AGENT = "GeoSettings:UserAgent";
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

        public static class SeedData
        {
            public const string ADMIN_CREATED_LOG = "SEED_DATA.ADMIN_CREATED_LOG";
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
            public const int MIN_REASON_LENGTH = 5;
            public const int MAX_REASON_LENGTH = 500;
            public const int MAX_SOCIAL_LENGTH = 100;
            public const int MAX_CATEGORY_NAME = 100;
        }

        public static class Columns
        {
            public const string CONTACT_PHONE = "Contact_Phone";
            public const string CONTACT_EMAIL = "Contact_Email";
            public const string ADDRESS_CITY = "Address_City";
            public const string ADDRESS_STREET = "Address_Street";
            public const string ADDRESS_HOUSE = "Address_HouseNumber";
            public const string CONTACT_WEBSITE = "Contact_Website";
            public const string CONTACT_TELEGRAM = "Contact_Telegram";
            public const string CONTACT_INSTAGRAM = "Contact_Instagram";
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
            public const string USER_NOT_FOUND = "ERRORS.USER_NOT_FOUND";
            public const string ALREADY_APPLIED_OR_ADMIN = "ERRORS.ALREADY_APPLIED_OR_ADMIN";
            public const string NO_CONTACTS_PROVIDED = "ERRORS.NO_CONTACTS_PROVIDED";
            public const string INVALID_EMAIL = "ERRORS.INVALID_EMAIL";
            public const string INVALID_TOKEN = "ERRORS.INVALID_TOKEN";
            public const string NOT_PENDING_ORGANIZER = "ERRORS.NOT_PENDING_ORGANIZER";
            public const string EVENT_ID_REQUIRED = "ERRORS.EVENT_ID_REQUIRED";
            public const string REASON_REQUIRED = "ERRORS.REASON_REQUIRED";
            public const string REASON_TOO_SHORT = "ERRORS.REASON_TOO_SHORT";
            public const string REASON_TOO_LONG = "ERRORS.REASON_TOO_LONG";
            public const string CATEGORY_NOT_EXIST = "ERRORS.CATEGORY_NOT_EXIST";
            public const string CATEGORY_ALREADY_EXISTS = "ERRORS.CATEGORY_ALREADY_EXISTS";
            public const string TG_TOKEN_MISSING = "ERRORS.TG_TOKEN_MISSING";

            // Артисты
            public const string PERFORMER_NOT_FOUND = "ERRORS.PERFORMER_NOT_FOUND";
            public const string PERFORMER_MERGE_FAILED = "ERRORS.PERFORMER_MERGE_FAILED";
            public const string PERFORMER_NAME_EMPTY = "ERRORS.PERFORMER_NAME_EMPTY";
            public const string PERFORMER_ALREADY_EXISTS = "ERRORS.PERFORMER_ALREADY_EXISTS";
        }

        public static class Success
        {
            public const string EVENT_CREATED = "SUCCESS.EVENT_CREATED";
            public const string REQUEST_APPROVED = "SUCCESS.REQUEST_APPROVED";
            public const string VERIFICATION_CODE_SENT = "SUCCESS.VERIFICATION_CODE_SENT";
            public const string APPLICATION_SUBMITTED = "SUCCESS.APPLICATION_SUBMITTED";
            public const string ORGANIZER_APPROVED = "SUCCESS.ORGANIZER_APPROVED";
            public const string ORGANIZER_REJECTED = "SUCCESS.ORGANIZER_REJECTED";
            public const string EVENT_APPROVED = "SUCCESS.EVENT_APPROVED";
            public const string EVENT_REJECTED = "SUCCESS.EVENT_REJECTED";
            public const string EVENT_DELETED = "SUCCESS.EVENT_DELETED";
            public const string EVENT_SENT_TO_FIX = "SUCCESS.EVENT_SENT_TO_FIX";
            public const string EVENT_VIP_TOGGLED = "SUCCESS.EVENT_VIP_TOGGLED";
            public const string CONTACTS_UPDATED = "SUCCESS.CONTACTS_UPDATED";

            // Артисты
            public const string PERFORMER_PENDING_MODERATION = "SUCCESS.PERFORMER_PENDING_MODERATION";
            public const string PERFORMER_VERIFIED = "SUCCESS.PERFORMER_VERIFIED";
            public const string PERFORMER_MERGED = "SUCCESS.PERFORMER_MERGED";

            public const string NOTIFICATION_EVENT_APPROVED_SUBJ = "NOTIFICATIONS.EVENT_APPROVED_SUBJECT";
            public const string NOTIFICATION_EVENT_APPROVED_BODY = "NOTIFICATIONS.EVENT_APPROVED_BODY";
            public const string NOTIFICATION_EVENT_APPROVED_VIP = "NOTIFICATIONS.EVENT_APPROVED_VIP";

            public const string NOTIFICATION_EVENT_REJECT_SUBJ = "NOTIFICATIONS.EVENT_REJECT_SUBJECT";
            public const string NOTIFICATION_EVENT_REJECT_BODY = "NOTIFICATIONS.EVENT_REJECT_BODY";

            public const string NOTIFICATION_EVENT_REJECTED_FINAL_SUBJ = "NOTIFICATIONS.EVENT_REJECTED_FINAL_SUBJECT";
            public const string NOTIFICATION_EVENT_REJECTED_FINAL_BODY = "NOTIFICATIONS.EVENT_REJECTED_FINAL_BODY";

            public const string NOTIFICATION_ORG_APPROVED_SUBJ = "NOTIFICATIONS.ORGANIZER_APPROVED_SUBJECT";
            public const string NOTIFICATION_ORG_APPROVED_BODY = "NOTIFICATIONS.ORGANIZER_APPROVED_BODY";

            public const string NOTIFICATION_ORG_REJECT_SUBJ = "NOTIFICATIONS.ORGANIZER_REJECT_SUBJECT";
            public const string NOTIFICATION_ORG_REJECT_BODY = "NOTIFICATIONS.ORGANIZER_REJECT_BODY";

            public const string PERFORMER_UPDATED = "SUCCESS.PERFORMER_UPDATED";
            public const string PERFORMER_DELETED = "SUCCESS.PERFORMER_DELETED";
        }

        public static class Others
        {
            public const string CONFIG_MISSING_JWT = "OTHERS.CONFIG_MISSING_JWT";
            public const string MIDDLEWARE_FATAL_LOG = "OTHERS.MIDDLEWARE_FATAL_LOG";
            public const string LOCATION_NOT_SPECIFIED = "OTHERS.LOCATION_NOT_SPECIFIED";
            public const string ORGANIZER_NOT_SPECIFIED = "OTHERS.ORGANIZER_NOT_SPECIFIED";
            public const string ANONIM = "OTHERS.ANONIM";
            public const string COMMENT_DELETE = "OTHERS.COMMENT_DELETE";
            public const string APPLICATION_SUCCESS = "OTHERS.APPLICATION_SUCCESS";
        }
    }
}