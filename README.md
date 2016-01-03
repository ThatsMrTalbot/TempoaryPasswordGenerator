[![Build Status](https://travis-ci.org/ThatsMrTalbot/TemporaryPasswordGenerator.svg?branch=master)](https://travis-ci.org/ThatsMrTalbot/TemporaryPasswordGenerator)
# Temporary Password Generator

This is a code experiment to create temporary time restricted passwords based on a secret.

The goal was to use an algorithm to generate the password so no backing database would be required.

To see the though processes behind the decisions see REASONING.md

Usage:
```c#
var generator = new PasswordGenerator(TimeSpan.FromSeconds(30), Encoding.UTF8.GetBytes("SomeSecret"));
var uid = "5bb9e9a2-7ee1-4b46-87b4-7b82cfcd8d51";

// To Generate
var password = generator.Generate(uid);

// To Validate
var valid = generator.Validate(uid, password);
```
