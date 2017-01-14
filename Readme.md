Cairn.
===================================

First of all, why Cairn? Well, a cairn is a human-made pile of stones used throughout history as landmarks and monuments. The idea being to draw a metaphor between a stack of stones that can be rearranged to form something more than the sum of its parts and a collection of functions and actions that can be rearranged and replaced, at will, to form an application or a process.

Cairn is basically a complete re-write/re-implementation of [inversion](https://github.com/guy-murphy/inversion-dev) taking the broad principles behind that project and going off in my own direction. 

Differences from Conclave/Inversion
==============

So the first thing to note is that Behaviours have been elevated. I was never overly comfortable with the idea that a developer implementing a Behaviour should need to concern themselves with checking a context to see if firstly, a value with a specific key name exists and secondly that value is the correct type needed within the Behaviour itself. I went round in circles and kept coming back to the idea that I like the idea of Behaviours, descrete, black box units of code that can be tested in isolation, but that they should expose some sort of contract that makes it obvious to the developer what these code units actually need in order to be able to execute. I very much had in the forefront of my mind the idea of - 'imagine being able to write a method and then use that method anywhere. 

Behaviours
----------------------------

In Cairn, Behaviours are now collections of Functions and Actions that is defined at a configuration level.

```csharp
public interface IFunction<T1, T2, TResult> {
	bool Condition(T1 arg1, T2 arg2);
	TResult Return(T1 arg1, T2 arg2);
}

public interface IAction<T1, T2, T3, T4> {
	bool Condition(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	void Execute(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}
```

So Functions and Actions are just interfaces that define a contract. Actions just 'do' stuff and returns nothing, Functions 'do' stuff and return a result. The developer is responsible for writing code that implements these interfaces. E.g.

```csharp
public class TransformData : IFunction<string[], string, string[]> {
	public bool Condition (string[] inputData, string extraData) {
		return true;
	}

	public string[] Return (string[] inputData, string extraData) {
		string[] processedData = new string[inputData.Length];
		for (int i = 0; i < inputData.Length; i++) {
			processedData[i] = String.Format(@"{0}:{1}", extraData, inputData[i]);
		}
		return processedData;
	}		
}
```

Configuration
----------------------------

So a Behaviour is defined in configuration files and basically consists of:

* Parameters - What paramaters need to exist in order for this Behaviour to fire.
* RespondsToMessages - Simply a list of strings that will trigger the Behaviour.
* ProcessesMessages - After the Behaviour has fired it can then itself fire messages and will fire these in order of how they are defined.
* Functions - A collection of functions that will execute and what parameters will be passed to them (you don't need to use all the parameters defined in the Behaviour itself).
* Actions - Same as functions but do not return a result.

```xml
<behaviour name="GetData">
	<respondsTo>
		<message>get-data</message>
	</respondsTo>
	<processes>
		<message>transform-data</message>
	</processes>
	<parameters>
		<parameter name="dummyDataComponent" />
	</parameters>
	<functions>
		<function type="Cairn.Harness.Behaviour.Functions.GetData, Cairn.Harness">
			<inputs>
				<parameterIndex>0</parameterIndex>
			</inputs>
			<output returnAsResult="False">data</output>
		</function>
	</functions>
</behaviour>
```

You need to define what paramaters will be used within the application, whether they come from the context or defined outright within the config.

```xml
<parameters>
	<parameter name="program" type="Cairn.Harness.Program, Cairn.Harness" parameterType="Context" />	
	<parameter name="data" type="System.String[]" parameterType="Context" />
	<parameter name="extraData" type="System.String" parameterType="Value" value="This is the value" />
	<parameter name="processedData" type="System.String[]" parameterType="Context" />
	<parameter name="dummyDataComponent" type="Cairn.Harness.Component.DummyDataComponent, Cairn.Harness" parameterType="Component" lifestyle="Transient">
		<dataSet type="System.String" value="Films" />
	</parameter>
</parameters>
```

There are 5 different types of Parameter:

* Context - denoting that the value will be found in the context when the time comes.
* Value - value types defined in the config, e.g. strings (yes I know they aren't value types but are treated as such), int's etc
* Components - Components are basically injected in at runtime like you would with Spring or Castle Windsor. I have basically implemented my own object creation code, it's still a bit rough and ready, it it works.
* List - inline list objects, still not tested massively.
* Dictionary - as List, still a bit raw.


To finish
==============

I don't have a lot of time to write these notes or explain my design decisions or the finer details of how stuff works, I'm hoping the Harness project can allow you to poke about and do that yourself. Just know that every opportunity has been made to keep things as efficient as possible as I know you'll likely have concerns regarding object creation and dynamic method invocation, it's fast...if I do say so myself, all cached.

