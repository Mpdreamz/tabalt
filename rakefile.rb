require 'rubygems'
require 'albacore'
require 'rake/clean'
require 'rexml/document'
require 'uuidtools'

TABALT_VERSION = "0.0.1"

OUTPUT = "bin"

CONFIGURATION = 'Release'
SHARED_ASSEMBLY_INFO = 'src/TabAlt/Properties/AssemblyInfo.cs'
SOLUTION_FILE = 'src/Tabalt.sln'
X64_SETUP_PROJECT = 'src/Tabalt.Setup/Tabalt.Setup.vdproj'
X86_SETUP_PROJECT = 'src/Tabalt.Setup - x86/Tabalt.Setup x86.vdproj'

#Yeah..., building using devenv because of vdproj's. 
PATH_TO_DEVENV = "D:/Program Files/Microsoft Visual Studio 10.0/Common7/IDE/devenv.exe"

# generate required new guids
ProductCodeGuid = UUIDTools::UUID.random_create.to_s.upcase
PackageCodeGuid = UUIDTools::UUID.random_create.to_s.upcase

Albacore.configure do |config|
    config.log_level = :verbose
    config.msbuild.use :net4
end

desc "Compiles solution and runs unit tests"
task :default => [:clean, :version, :prepare_setups, :setupx86, :setupx64, :publish, :addlauchoptionx64, :addlauchoptionx86]

#Add the folders that should be cleaned as part of the clean task
CLEAN.include(OUTPUT)
CLEAN.include(FileList["src/**/#{CONFIGURATION}"])

desc "Update shared assemblyinfo file for the build"
assemblyinfo :version => [:clean] do |asm|
    asm.version = TABALT_VERSION
    asm.company_name = "Tabalt"
    asm.product_name = "Tabalt"
    asm.title = "Tabalt"
    asm.description = "An alternative alt tab implementation for power users"
    asm.copyright = "Copyright (C) Martijn Laarman and contributors"
    asm.output_file = SHARED_ASSEMBLY_INFO
end

task :prepare_setups do 
    file_names = [X64_SETUP_PROJECT, X86_SETUP_PROJECT]

    file_names.each do |file_name|
        contents = File.read(file_name)
        File.open(file_name, 'w') do |file|
            file.puts contents
              .gsub(/("ProductVersion" = "8:)\d+\.\d+\.\d+"/, "\\1#{TABALT_VERSION}\"")
              .gsub(/("ProductCode" = "8:){[^}]+}"/, "\\1{#{ProductCodeGuid}}\"")
              .gsub(/("PackageCode" = "8:){[^}]+}"/, "\\1{#{PackageCodeGuid}}\"")
      end    end
end

exec :setupx64 do |cmd|
    cmd.command = PATH_TO_DEVENV
    cmd.parameters = "#{SOLUTION_FILE} /rebuild \"Release|x64\""
end
exec :setupx86 do |cmd|
    cmd.command = PATH_TO_DEVENV
    cmd.parameters = "#{SOLUTION_FILE} /rebuild \"Release|x86\""
end

exec :addlauchoptionx64 do |cmd|
    cmd.command = "cscript"
    cmd.parameters = "\"src/Scripts/EnableLaunchApplication.js\" \"#{OUTPUT}/Tabalt-#{TABALT_VERSION}-setup-x64.msi\""
end

exec :addlauchoptionx86 do |cmd|
    cmd.command = "cscript"
    cmd.parameters = "\"src/Scripts/EnableLaunchApplication.js\" \"#{OUTPUT}/Tabalt-#{TABALT_VERSION}-setup-x86.msi\""
end

desc "Gathers output files and copies them to the output folder"
task :publish do
    Dir.mkdir(OUTPUT)
    FileUtils.mv("src/Tabalt.Setup/Release/Tabalt.Setup.msi", "#{OUTPUT}/Tabalt-#{TABALT_VERSION}-setup-x64.msi")
    FileUtils.mv("src/Tabalt.Setup - x86/Release/Tabalt.Setup.msi", "#{OUTPUT}/Tabalt-#{TABALT_VERSION}-setup-x86.msi")

    

end


#TODO:

#DONE:
