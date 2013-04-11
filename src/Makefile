
CSC=mcs #cscc
EXE=mono #ilrun
BIN= bin
ASS= -pkg:gtk-sharp-2.0 -r:System.Drawing.dll -pkg:gtk-dotnet-2.0
RES= -resource:gb.ico,gb_icon
ROMTEST=../gbdev/romtest/sprite.gb
DOC= csgbe.xml
OPTS= -win32icon:gb.ico -doc:${DOC}
FECHA=`date +%Y_%m_%d_%H.%M`

all: clean compilar run

compilar: 
	@echo Compiling...
	@${CSC} -unsafe -out:$(BIN)/csgbe.exe ${ASS} ${RES} -recurse:*.cs ${OPTS}

doc:
	# still nothing

zip: clean
	@echo Backup [$(FECHA)]
	@cd .. && rm -f csgbe_$(FECHA).zip && zip -9 -r csgbe_$(FECHA).zip csgbe

sync: zip
	@echo Connecting...
	@scp -P 6112 ../csgbe_$(FECHA).zip victor@denibol.com:Proyectos/csgbe/

clean:
	@echo Cleaning...
	@rm -rf ${BIN}/*.exe
	@rm -rf ${DOC}
#	@rm -rf ${BIN}/*.dll

run:
	${EXE} ${BIN}/csgbe.exe

test: clean compilar
	${EXE} ${BIN}/csgbe.exe ${ROMTEST}
