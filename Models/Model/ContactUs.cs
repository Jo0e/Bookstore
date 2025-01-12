using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Model
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RequestType Request { get; set; }
        public bool IsReadied { get; set; } = false;
        public string? AdditionalPhoneNumber { get; set; }
        public string? UserImgRequest { get; set; }
        public int? CommentId { get; set; }
        public Comment Comment { get; set; }

        public required string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
    public enum RequestType
    {
        Complaint,
        Suggestion,
        AuthorRequest,
    }
}
