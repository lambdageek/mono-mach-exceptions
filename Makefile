native_library=libexception-negotiation-native.dylib
native_source=exception-negotiation-native.c
managed_executable=exception-negotiation.exe
managed_source=exception-negotiation.cs

arch = x86_64
#arch = i386
HOST = HOST_AMD64
#HOST = HOST_I386

DEBUG_FLAGS=-g

all : $(native_library) $(managed_executable)

$(native_library) : $(native_source)
	$(CC) $(DEBUG_FLAGS) -arch $(arch) -D$(HOST) $(native_source) -shared -o $(native_library)

$(managed_executable) : $(managed_source)
	csc $(managed_source)

.PHONY: clean

clean:
	-rm -f $(managed_executable) $(native_library)
	-rm -rf $(native_library).dSYM
