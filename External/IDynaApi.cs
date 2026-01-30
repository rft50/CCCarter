using System.Collections.Generic;
using Nickel;

namespace Carter.External;

public interface IDynaApi
{
	IDeckEntry DynaDeck { get; }

	IStatusEntry TempNitroStatus { get; }
}