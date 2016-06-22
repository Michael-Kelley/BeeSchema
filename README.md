![BeeSchema Logo](beeschema.png)

# BeeSchema
## Binary Schema Library for C&#35;

BeeSchema allows you to define the structure of a binary file using a simple schema (&#42;.bee) file, then read the file in a much more human way.  No more BinaryReader or manually reading data from Streams.

*NOTE: BeeSchema is still in development, so things may change and other things might not be too optimised.  Also, BeeSchema was designed as a replacement for the limited structure definition support in [Ostara](https://github.com/Epidal/Ostara).  As such, some features that you require may not be supported.*


## Schema Files
The structure of a binary file to be parsed by BeeSchema is defined in a simple text file (usually with the extension &#42;.bee) that uses a custom language designed specifically for this purpose.  These schemas can then be loaded by BeeSchema, and later used to parse binary files and turn them into more human-readable data.
The basic structure of any schema file consists of optional custom data types and a **schema** block that represents the entry point of the schema file.  This block is where BeeSchema will begin its parsing of binary files.  For example, a schema file for a simple binary file that consists of a 32-bit integer followed by an 8-bit integer might look like this:
```elm
schema {
  first_var : Int;
  next_var  : Byte;
}
```


### Primitive Data Types
BeeSchema supports a number of primitive data types.  The following table lists these types, along with a short description and their size in bytes.

*NOTE: Primitive type names are case-insensitive, so `an_int : int` is exactly the same as `an_int : Int`*

Type | Size (bytes) | Description
--- | --- | ---
bool | 1 | A boolean value (true or false)
byte | 1 | An unsigned 8-bit integer
sbyte | 1 | A signed 8-bit integer
ushort | 2 | An unsigned 16-bit integer
short | 2 | A signed 16-bit integer
uint | 4 | An unsigned 32-bit integer
int | 4 | A signed 32-bit integer
ulong | 8 | An unsigned 64-bit integer
long | 8 | A signed 64-bit integer
float | 4 | A 32-bit floating point integer
double | 8 | A 64-bit floating point integer
ipaddress | 4 | A 32-bit representation of an IP address, where each byte represents a portion of the address (eg. 0x04030201 = 1.2.3.4)
epoch | 4 | A 32-bit value representing a date and time in Unix format (seconds since 01/01/1970)

Arrays are also supported.  They can be declared like so:
```elm
a_byte_array  : Byte[7];
```
You can use a previously-declared variable for the array length:
```elm
some_int      : Int;
another_array : Float[some_int];
```
and even perform arithmetic operations in the length specifier:
```elm
some_other_int        : Int;
oh_look_another_array : Char[some_other_int / 4];
```
*NOTE: Referencing variables inside a custom type is not currently supported, but is planned for the future.  Absolute and relative pointers may also be added.*


### Custom Data Types
BeeSchema also supports custom data types in the form of structures, enumerations and bitfields.

#### Structures
Structures can be used to define the structure of a block of data.  They are useful for when your data file contains several blocks of data that share the same format.  Structures are defined with the **struct** keyword, and can contain any type and number of other variables, including other previously defined structures, enumerations or bitfields:
```elm
struct MyStruct {
  my_int  : Int;
}
```
Once defined, structures can be used like any other type:
```elm
my_struct : MyStruct;
```
When parsing structures, BeeSchema will return a collection of results, with each one representing a separate variable inside the struct (the same applies to bitfields and arrays).

#### Enumerations
Enumerations can be used for variables that have a fixed number of values that you might want to associate names with.  By default, values in an enumeration start at zero and increment by 1, but they can also be manually assigned.  Enumerations are defined with the **enum** keyword and must always declare a base type so that BeeSchema knows the size of the data to read:
```elm
enum MyEnum : Int {
  AValueThatIsZero,
  ThisValueIsOne,
  ThisIsTwo,
  ButThisIsSeven = 7
  AndNowThisIsEight
}
```

#### Bitfields
Bitfields are used to represent data that packs several values together.  They are defined with the **bitfield** keyword and, like enumerations, must declare a base type.  Additionally, each variable inside a bitfield must specify its size in bits:
```elm
bitfield MyBitfield : Byte {
  first_bit             : 1;
  second_to_fourth_bits : 3;
  remaining_bits        : 4;
}
```


### Conditionals and Loops
BeeSchema has basic support for conditionals and loops, including **if**, **unless**, **while**, and **until**.  Conditions for these can consist of references to previously-defined variables, macros, and comparison operators (!, ==, !=, <, >, <=, and >=).  Conditions can be chained together using OR (||) or AND (&&):
```elm
if (some_value > 5 && another_value <= 42) {
  an_int  : Int;
  a_short : Short;
}
```
The following table lists the currently supported conditionals and loops and a brief description of their purpose.

Keyword | Description
--- | ---
if | If the condition is met, any variables in the associated block will be evaluated
unless | Like **if**, but only evaluates the associated block if the condition is **not** met
while | Evaluates the variables in the associated block while the condition is true
until | Evaluates the variables in the associated block while the condition is false


### Macros
BeeSchema supports a few macros that provide information about the current binary file being parsed.  They can be used as conditions and in array length specifiers.  Macros are prepended with the **@** character and evaluated by BeeSchema while binary data is being parsed.  The following table lists the currently supported macros and what they represent.

Macro | Description
--- | ---
@eof | Evaluates to **true** if the end of the binary data has been reached
@size | Evaluates to the size of the binary data
@pos | Evaluates to the current position of the parser in the binary data


### Comments
Schema files support both single-line and block (multi-line) comments.  When BeeSchema discovers a comment, it will attach it to the most recently-declared variable.  This is useful for including annotations that you would like to display in your program along with the data.

#### Single-Line Comments
Single-line comments are declared by the **&#35;** character, followed by the comment itself:
```elm
a_string  : String;   # This is a comment!
```

#### Block (Multi-Line) Comments
Multi-Line comments (otherwise known as "block comments") are declared by wrapping the comment text in double-hashes (ie. two **&#35;** characters):
```
## This is
a multiline
comment.
##
```


### Including Schema Files
BeeSchema allows you to include other schema files inside your schema file.  This will import all of the defined custom data types, but will ignore the **schema** block inside that file:
```elm
include some_schema_file.bee;
```
This can be useful when several data formats share common types as it prevents you from having to repeat code and helps reduce the size of schema files.


### Example Schema File
Here's an example schema file that demonstrates all of the current features of BeeSchema:
```elm
# We can include external files so that we can use types defined in them
include somefile.bee;

# This is a single-line comment

##
	This is a
	multi-line
	comment.
##

struct SomeStruct {
	a_bool		: bool;
	a_byte		: byte;
	an_sbyte	: sbyte;
	a_ushort	: ushort;
	a_short		: short;
	an_int		: int;
	a_uint		: uint;
	a_long		: long;
	a_ulong		: ulong
	a_float		: float;
	a_double	: double;
	a_string	: string;
	an_ip		: ipaddress;
	a_timestamp	: epoch
}

# Enum and bitfield definitions require a base type
enum SomeEnum : byte {
	SomeValue1,
	SomeValue2,
	AnotherValue = 10,
	YetAnotherValue
}

bitfield SomeBitfield : byte {
	first_bit	: 1;
	next_3_bits	: 3;
	last_4_bits	: 4;
}

schema {
	a_struct    : SomeStruct;   # Variables can be annotated by using a comment
	an_enum     : SomeEnum;
	a_bitfield  : SomeBitfield;
	length      : Int;
	an_array    : Char[length];
	an_array2   : Int[4 * 2];
	an_array3   : Byte[@size - @pos - 32];
	int_1,
	int_2,
	int_3       : Int;  # Multiple variables of the same type can be declared by separating them with a comma

	until(@eof) {
		some_var    : Float;
		another_var : Epoch;
	}
}
```


## Using BeeSchema
To use BeeSchema, create an instance of the **Schema** class using the static **FromFile()** or **FromText()** methods.  You can then parse a binary file by passing the filename, a byte array, or a Stream object to the instance's **Parse()** method.  Please see [the Example project](Example/Program.cs) for a simple example on how to use BeeSchema and the resulting parsed data.
