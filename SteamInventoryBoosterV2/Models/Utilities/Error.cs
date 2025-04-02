namespace SteamInventoryBoosterV2.Models.Utilities
{
    public class Error
    {
        public string Message { get; set; }
        public ErrorCode Code { get; set; }

        public Error(string message, ErrorCode code)
        {
            Message = message;
            Code = code;
        }

        public enum ErrorCode
        {
            DuplicateItem,
            CookiesNotFound,
            InternalError,
            SteamItemNotFound,
            InvalidParameter,
            InventoryRetrievalFailed
        }

        // General Errors
        public static Error InternalError => new Error("An internal error occurred.", ErrorCode.InternalError);

        // Specific errors for SteamItem
        public static Error SteamItemAlreadyExists => new Error("SteamItem with this link already exists!", ErrorCode.DuplicateItem);
        public static Error SteamItemNotFound => new Error("SteamItem with that ID does not exist!", ErrorCode.SteamItemNotFound);
        public static Error SortColumnNotDefined => new Error("The column name for sorting is not defined!", ErrorCode.InvalidParameter);

        // Specific errors for the bot manager
        public static Error CookiesNotFound => new Error("Cookies could not be found!", ErrorCode.CookiesNotFound);
        public static Error InventoryRetrievalFailed => new Error("Failed to retrieve inventory information!", ErrorCode.InventoryRetrievalFailed);
    }
}
