namespace Grammophone.Ennoun.Web.Controllers
{
	/// <summary>
	/// The state of the inference engine used by <see cref="InferenceSession"/> instances.
	/// </summary>
	public enum InferenceEngineState : int
	{
		/// <summary>
		/// The engine has not been started.
		/// </summary>
		NotStarted,

		/// <summary>
		/// The engine is starting but it is not ready yet.
		/// </summary>
		Starting,

		/// <summary>
		/// The engine is fully operational.
		/// </summary>
		Started,

		/// <summary>
		/// The start of the engine failed.
		/// </summary>
		Error
	}
}