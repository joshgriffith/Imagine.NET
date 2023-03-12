## Introduction
Imagine.NET is a .NET Core library for the generation and transformation of data using OpenAI's GPT API. It seamlessly works with .NET types and data structures to take full advantage of GPT's excellent zero-shot semantic understanding and completion, enabling simple but powerful data processing paradigms.

## Why?
Most software works with structured data with known types at compile-time. However, it can be challenging to get such structured data in and out of GPT prompt chains. Imagine.NET lets you focus on describing your domain model in a fluent and declarative manner that is both familiar and productive. This project is used to power the 'imagination' and reasoning of AI agents like [Cheevly](https://www.cheevly.com/).

## Overview
Imagine.NET offers two types of processing pipelines.

### Type + Prompt
Provide a .NET Type and a text prompt to get back a list of data that conforms to the prompt.
  
### List + Type + Prompt
Provide a list of input data, a .NET Type and a text prompt and get back a new list of data derived from the list and prompt.
  
![Diagram](design.png?raw=true "Diagram")
