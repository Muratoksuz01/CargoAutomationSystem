namespace CargoAutomationSystem.Helpers
{
    public static class MaskingHelper
    {
        public static string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length < 2)
                return name;

            var first = name[0];
            var last = name[name.Length - 1];
            return $"{first}{"*".PadLeft(name.Length - 2, '*')}{last}";
        }

        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 4)
                return phone;

            return $"{new string('*', phone.Length - 4)}{phone[^4..]}";
        }
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return email;

            var parts = email.Split('@');
            if (parts.Length < 2) return email;

            var username = parts[0];
            var domain = parts[1];

            var visibleUsername = username.Substring(0, 1) + new string('*', username.Length - 2) + username.Substring(username.Length - 1, 1);
            return visibleUsername + "@" + domain;
        }
    }
}
