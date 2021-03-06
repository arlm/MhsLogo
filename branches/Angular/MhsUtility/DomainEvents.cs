﻿using System;
using System.Collections.Generic;

namespace MhsUtility
{
	//
	// Udi Dahan: http://www.udidahan.com/2009/06/14/domain-events-salvation/
	//
	public class DomainEvents
	{
		[ThreadStatic] private static List<Delegate> actions;

		// Registers a callback for the given domain event
		public static void Register<T>(Action<T> callback) where T : IDomainEvent
		{
			if (actions == null)
			{
				actions = new List<Delegate>();
			}
			actions.Add(callback);
		}

		// Clears callbacks passed to Register on the current thread
		public static void ClearCallbacks()
		{
			actions = null;
		}

		// Raises the given domain event
		public static void Raise<T>(T args) where T : IDomainEvent
		{
			if (actions != null)
			{
				foreach (Delegate action in actions)
				{
					if (action is Action<T>)
					{
						((Action<T>) action)(args);
					}
				}
			}
		}
	}
}