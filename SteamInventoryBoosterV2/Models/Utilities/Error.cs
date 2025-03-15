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
            DuplicateItem
        }

        // General Errors
        //public static Error InternalServerError => new Error("Erro interno no servidor", ErrorCode.InternalError);
        //public static Error NotFound => new Error("Item não encontrado", ErrorCode.NotFound);
        //public static Error InvalidLink => new Error("O link fornecido é inválido", ErrorCode.InvalidLink);

        // Erros Específicos para SteamItem
        public static Error SteamItemAlreadyExists => new Error("SteamItem with this link already exists!", ErrorCode.DuplicateItem);
        //public static Error SteamItemCreationFailed => new Error("Falha ao criar SteamItem", ErrorCode.CreationFailed);
    }
}
