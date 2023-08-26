using Lombok.NET;
using RESTful_1.Enumeration;

namespace RESTful_1.Dto
{
    [AllArgsConstructor(MemberType = MemberType.Property, AccessTypes = AccessTypes.Public)]
    public partial class AuthorizeResponseDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
