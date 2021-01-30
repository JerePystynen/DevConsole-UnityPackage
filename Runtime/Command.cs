namespace Exodia
{
    public abstract class Command
    {
        ///<summary>The name used to call the command.</summary>
        public abstract string[] Names();
        
        ///<summary>The function done by the command.</summary>
        public abstract string Function(params object[] input);
    }
}