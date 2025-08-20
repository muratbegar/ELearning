using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IskoopDemo.Shared.Application.DTOs
{
    // Command Response (for operations that don't return data)
    public class CommandResponse : BaseResponse
    {
        public string ResourceId { get; set; }
        public string ResourceLocation { get; set; }

        public CommandResponse() : base() { }

        public CommandResponse(bool isSuccess, string message = null) : base(isSuccess, message) { }

        public CommandResponse(bool isSuccess, string message, string resourceId) : this(isSuccess, message)
        {
            ResourceId = resourceId;
        }

        public static CommandResponse Success(string message = "Operation completed successfully")
        {
            return new CommandResponse(true, message);
        }

        public static CommandResponse Success(string message, string resourceId)
        {
            return new CommandResponse(true, message, resourceId);
        }

        public static CommandResponse Created(string resourceId, string resourceLocation = null)
        {
            return new CommandResponse(true, "Resource created successfully", resourceId)
            {
                ResourceLocation = resourceLocation
            };
        }

        public static CommandResponse Updated(string resourceId)
        {
            return new CommandResponse(true, "Resource updated successfully", resourceId);
        }

        public static CommandResponse Deleted(string resourceId)
        {
            return new CommandResponse(true, "Resource deleted successfully", resourceId);
        }

        public new static CommandResponse Failure(string message = "Operation failed")
        {
            return new CommandResponse(false, message);
        }

        public new static CommandResponse Failure(ApiError error)
        {
            return new CommandResponse(false, error.Message) { Error = error };
        }
    }
}
