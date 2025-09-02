namespace UserManagementApp.Exceptions
{
    public class UserValidationException : Exception
    {
        public string? PropertyName { get; }

        public UserValidationException(string message, string propertyName = null) 
            : base(message)
        {
            PropertyName = propertyName;
        }

        public UserValidationException(string message, Exception innerException, string propertyName = null) 
            : base(message, innerException)
        {
            PropertyName = propertyName;
        }
    }
}