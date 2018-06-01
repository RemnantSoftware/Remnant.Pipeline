
using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
    public interface IRegisterObserver
    {
			/// <summary>
			/// Register an additional observer for the pipeline
			/// </summary>
			/// <typeparam name="TObserver">The generic observer type</typeparam>
			/// <returns>Returns interface to continue registration</returns>
	    IRegisterObserver AndObserver<TObserver>() where TObserver : Observer, new();
    }
}
