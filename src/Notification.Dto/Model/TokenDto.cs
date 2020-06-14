using System;

namespace Notification.Dto.Model
{
    public class TokenDto
    {

        public string TokenRefresh { get; set; }
        public string TokenJwt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool Invalidated { get; set; }
        public string JwtId { get; set; }
    }
}
