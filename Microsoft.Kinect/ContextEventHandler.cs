using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Microsoft.Kinect
{
	/// <summary>
	/// Wrapper that holds a collection of event handlers for a specific type and associates
	/// a context with them.
	/// </summary>
	/// <typeparam name="T">EventArgs for the specific event.</typeparam>
	internal class ContextEventHandler<T> : IDisposable where T : EventArgs
	{
		/// <summary>
		/// The possible methods for sending data to the synchronization context.
		/// </summary>
		internal enum ContextSynchronizationMethod
		{
			/// <summary>
			/// The send method is used to pass synchronous message to the context.
			/// </summary>
			Send,
			/// <summary>
			/// The post method is used to pass an asynchronous message to the context.
			/// </summary>
			Post
		}

		/// <summary>
		/// Container class to hold event handler, sender and args so that it can be
		/// marshalled using the Synchronization context.
		/// </summary>
		private class ContextEventHandlerArgsWrapper
		{
			/// <summary>
			/// Gets the associated event handler.
			/// </summary>
			public EventHandler<T> Handler { get; private set; }

			/// <summary>
			/// Gets the sending object.
			/// </summary>
			public object Sender { get; private set; }

			/// <summary>
			/// Gets the event arguments object.
			/// </summary>
			public T Args { get; private set; }

			/// <summary>
			/// Initializes a new instance of the ContextEventHandlerArgsWrapper class.
			/// </summary>
			/// <param name="handler">The event handler.</param>
			/// <param name="sender">The sending object.</param>
			/// <param name="args">The argument object.</param>
			public ContextEventHandlerArgsWrapper(EventHandler<T> handler, object sender, T args)
			{
				Handler = handler;
				Sender = sender;
				Args = args;
			}
		}

		/// <summary>
		/// Identifies a SynchronizationContext taking the potential that it is a DispatcherSynchronizationContext
		/// into account.  In .NET 4.5, WPF made a breaking change to that class that affects stability of the
		/// actual value of a SynchronizationContext in WPF apps.  While the actual SynchronizationContext instance
		/// may change, the internal referenced Dispatcher will not, so we use this for determining identity when available.
		/// </summary>
		private class SynchronizationContextIdentifier
		{
			/// <summary>
			/// Name of the internal field in DispatcherSynchronizationContext to reflect on when
			/// comparing SynchronizationContextIdentifiers.
			/// </summary>
			private const string DispatcherFieldName = "_dispatcher";

			/// <summary>
			/// The unqualified name of the DispatcherSynchronizationContext type.
			/// </summary>
			private const string DispatcherSynchronizationContextName = "DispatcherSynchronizationContext";

			/// <summary>
			/// Storage for the Dispatcher object, if one exists.
			/// </summary>
			private object _dispatcherObject;

			/// <summary>
			/// Gets the SynchronizationContext held by the SynchronizationContextIdentifier.
			/// </summary>
			public SynchronizationContext Context { get; private set; }

			/// <summary>
			/// Initializes a new instance of the SynchronizationContextIdentifier class.
			/// </summary>
			/// <param name="context">The SynchronizationContext to extract identity for.</param>
			public SynchronizationContextIdentifier(SynchronizationContext context)
			{
				Context = context;
				if (object.ReferenceEquals(context, null))
				{
					return;
				}
				Type type = context.GetType();
				if ("DispatcherSynchronizationContext" == type.Name)
				{
					FieldInfo field = type.GetField("_dispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
					if (field != null)
					{
						_dispatcherObject = field.GetValue(context);
					}
				}
			}

			/// <summary>
			/// Gets the hashcode of the object.
			/// </summary>
			/// <returns>Hashcode of the SynchronizationContextIdentifier.</returns>
			public override int GetHashCode()
			{
				if (object.ReferenceEquals(_dispatcherObject, null))
				{
					if (!object.ReferenceEquals(Context, null))
					{
						return Context.GetHashCode();
					}
					return 0;
				}
				return _dispatcherObject.GetHashCode();
			}

			/// <summary>
			/// Returns whether this SynchronizationContextIdentifier is equivalent to obj.
			/// </summary>
			/// <param name="obj">The object to compare for equivalence.</param>
			/// <returns>True if this SynchronizationContextIdentifier is equivalent to obj, false otherwise.</returns>
			public override bool Equals(object obj)
			{
				SynchronizationContextIdentifier synchronizationContextIdentifier = obj as SynchronizationContextIdentifier;
				if (object.ReferenceEquals(synchronizationContextIdentifier, null))
				{
					return false;
				}
				return Equals(synchronizationContextIdentifier);
			}

			/// <summary>
			/// Returns whether this SynchronizationContextIdentifier is equivalent to obj.
			/// </summary>
			/// <param name="contextId">The object to compare for equivalence.</param>
			/// <returns>True if this SynchronizationContextIdentifier is equivalent to obj, false otherwise.</returns>
			public bool Equals(SynchronizationContextIdentifier contextId)
			{
				if (object.ReferenceEquals(contextId, null))
				{
					return false;
				}
				if (_dispatcherObject == null && contextId._dispatcherObject == null)
				{
					return Context == contextId.Context;
				}
				return _dispatcherObject == contextId._dispatcherObject;
			}

			/// <summary>
			/// This method compares two SynchronizationContextIdentifiers.
			/// </summary>
			/// <param name="contextId1">The first SynchronizationContextIdentifier to compare.</param>
			/// <param name="contextId2">The second SynchronizationContextIdentifier to compare.</param>
			/// <returns>Returns true if they are equal, false otherwise.</returns>
			public static bool operator ==(SynchronizationContextIdentifier contextId1, SynchronizationContextIdentifier contextId2)
			{
				if (object.ReferenceEquals(contextId1, null) || object.ReferenceEquals(contextId2, null))
				{
					return false;
				}
				return contextId1.Equals(contextId2);
			}

			/// <summary>
			/// This method compares two SynchronizationContextIdentifiers.
			/// </summary>
			/// <param name="contextId1">The first SynchronizationContextIdentifier to compare.</param>
			/// <param name="contextId2">The second SynchronizationContextIdentifier to compare.</param>
			/// <returns>Returns false if they are equal, true otherwise.</returns>
			public static bool operator !=(SynchronizationContextIdentifier contextId1, SynchronizationContextIdentifier contextId2)
			{
				if (object.ReferenceEquals(contextId1, null) || object.ReferenceEquals(contextId2, null))
				{
					return true;
				}
				return !contextId1.Equals(contextId2);
			}
		}

		/// <summary>
		/// Container class to associate an event handler with a context so that they
		/// act as a single key in a list.
		/// </summary>
		private class ContextHandlerPair
		{
			/// <summary>
			/// Gets the associated synchronization context identifier.
			/// </summary>
			public SynchronizationContextIdentifier ContextId { get; private set; }

			/// <summary>
			/// Gets the associated event handler.
			/// </summary>
			public EventHandler<T> Handler { get; private set; }

			/// <summary>
			/// Initializes a new instance of the ContextHandlerPair class.
			/// </summary>
			/// <param name="handler">The target handler.</param>
			/// <param name="contextId">The target context identifier.</param>
			public ContextHandlerPair(EventHandler<T> handler, SynchronizationContextIdentifier contextId)
			{
				Handler = handler;
				ContextId = contextId;
			}
		}

		/// <summary>
		/// This holds the list of context to handler mappings.
		/// </summary>
		private readonly ThreadSafeList<ContextHandlerPair> _actualHandlers;

		/// <summary>
		/// This is the method used process the message by the synchronization context.
		/// </summary>
		private readonly ContextSynchronizationMethod _method;

		/// <summary>
		/// This keeps track of the disposed state of the object.
		/// </summary>
		private bool _isDisposed;

		/// <summary>
		/// Gets a value indicating whether this wrapper has any actual handlers registered.
		/// </summary>
		public bool HasHandlers => _actualHandlers.Count > 0;

		/// <summary>
		/// Initializes a new instance of the ContextEventHandler class.
		/// </summary>
		/// <param name="method">Determines whether the context will use Post or Send. Default is Post.</param>
		public ContextEventHandler(ContextSynchronizationMethod method = ContextSynchronizationMethod.Post)
		{
			_isDisposed = false;
			_method = method;
			_actualHandlers = new ThreadSafeList<ContextHandlerPair>();
		}

		/// <summary>
		/// Adds an event handler and associates it with the current context.
		/// </summary>
		/// <param name="originalHandler">The new event to add to the list of handlers.</param>
		public void AddHandler(EventHandler<T> originalHandler)
		{
			if (originalHandler != null)
			{
				_actualHandlers.Add(new ContextHandlerPair(originalHandler, new SynchronizationContextIdentifier(SynchronizationContext.Current)));
			}
		}

		/// <summary>
		/// Removes the event handler associated with the current context.
		/// </summary>
		/// <param name="originalHandler">The event to remove from the list of handlers.</param>
		public void RemoveHandler(EventHandler<T> originalHandler)
		{
			SynchronizationContextIdentifier synchronizationContextIdentifier = new SynchronizationContextIdentifier(SynchronizationContext.Current);
			ContextHandlerPair contextHandlerPair = null;
			foreach (ContextHandlerPair item in (IEnumerable<ContextHandlerPair>)_actualHandlers)
			{
				EventHandler<T> handler = item.Handler;
				SynchronizationContextIdentifier contextId = item.ContextId;
				if (synchronizationContextIdentifier == contextId && handler == originalHandler)
				{
					contextHandlerPair = item;
					break;
				}
			}
			if (contextHandlerPair != null)
			{
				_actualHandlers.Remove(contextHandlerPair);
			}
		}

		/// <summary>
		/// Invokes all registered event handlers.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		/// <param name="eventArgs">The event arguments to be passed to the handler.</param>
		public void Invoke(object sender, T eventArgs)
		{
			if (!HasHandlers)
			{
				return;
			}
			foreach (ContextHandlerPair item in (IEnumerable<ContextHandlerPair>)_actualHandlers)
			{
				EventHandler<T> handler = item.Handler;
				SynchronizationContext context = item.ContextId.Context;
				if (context == null)
				{
					handler(sender, eventArgs);
				}
				else if (_method == ContextSynchronizationMethod.Post)
				{
					context.Post(SendOrPostDelegate, new ContextEventHandlerArgsWrapper(handler, sender, eventArgs));
				}
				else if (_method == ContextSynchronizationMethod.Send)
				{
					context.Send(SendOrPostDelegate, new ContextEventHandlerArgsWrapper(handler, sender, eventArgs));
				}
			}
		}

		/// <summary>
		/// This method marks the object as disposed.
		/// </summary>
		public void Dispose()
		{
			_isDisposed = true;
			_actualHandlers.Clear();
		}

		/// <summary>
		/// Internal handler that matches the delegates for SynchronizationContext.Post/Send.
		/// </summary>
		/// <param name="state">State packed as ContextEventHandlerArgsWrapper ( handler + sender + args ).</param>
		private void SendOrPostDelegate(object state)
		{
			if (!_isDisposed)
			{
				ContextEventHandlerArgsWrapper contextEventHandlerArgsWrapper = (ContextEventHandlerArgsWrapper)state;
				contextEventHandlerArgsWrapper.Handler(contextEventHandlerArgsWrapper.Sender, contextEventHandlerArgsWrapper.Args);
			}
		}
	}
}
