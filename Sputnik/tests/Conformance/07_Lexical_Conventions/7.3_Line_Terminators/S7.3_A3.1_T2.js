// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/** * @name: S7.3_A3.1_T2; * @section: 7.3, 7.4; * @assertion: Single line comments can not contain LINE FEED (U+000A) inside; * @description: Insert LINE FEED (\u000A) into begin of single line comment; * @negative */// CHECK#1eval("//\u000A single line comment");
