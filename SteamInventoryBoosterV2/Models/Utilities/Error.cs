using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            InternalError
        }

        // General Errors
        public static Error InternalError => new Error("An internal error occurred.", ErrorCode.InternalError);

        // Erros Específicos para SteamItem
        public static Error SteamItemAlreadyExists => new Error("SteamItem with this link already exists!", ErrorCode.DuplicateItem);
        public static Error CookiesNotFound => new Error("Cookies could not be found!", ErrorCode.CookiesNotFound);

    }
}
