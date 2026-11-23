namespace basic_ecommerce_platform.Exceptions
{
    public class BadRequestException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public BadRequestException(IEnumerable<string> errors) : base("Request failed validation")
        {
            Errors = errors;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
