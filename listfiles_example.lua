-- Example usage of the listfiles() function
-- This function securely lists files in directories within the plugin path
-- All paths are relative to the plugin directory (pluginPath)

-- Example 1: List all files in the plugin root directory
print("=== Files in plugin root directory ===")
local files = listfiles("")  -- Empty string or "." for root
if files then
    for i = 1, #files do
        print("File " .. i .. ": " .. files[i])
    end
else
    print("No files found or directory doesn't exist")
end

-- Example 2: List files in a subdirectory
print("\n=== Files in 'scripts' subdirectory ===")
local scriptFiles = listfiles("scripts")  -- Relative to pluginPath
if scriptFiles then
    for i = 1, #scriptFiles do
        print("Script file " .. i .. ": " .. scriptFiles[i])
    end
else
    print("No script files found or 'scripts' directory doesn't exist")
end

-- Example 3: Check if specific files exist
print("\n=== Checking for specific files ===")
local allFiles = listfiles(".")
if allFiles then
    local foundFiles = {}
    for i = 1, #allFiles do
        local filename = allFiles[i]
        -- Check for .lua files
        if string.match(filename, "%.lua$") then
            table.insert(foundFiles, filename)
        end
    end

    if #foundFiles > 0 then
        print("Found " .. #foundFiles .. " Lua files:")
        for i = 1, #foundFiles do
            print("  " .. foundFiles[i])
        end
    else
        print("No Lua files found")
    end
end

-- Example 4: Process files with readfile()
print("\n=== Processing Lua files ===")
local luaFiles = listfiles("")  -- List files in plugin root
if luaFiles then
    for i = 1, #luaFiles do
        local filename = luaFiles[i]
        if string.match(filename, "%.lua$") then
            print("Processing: " .. filename)
            local content = readfile(filename)  -- filename is already relative to pluginPath
            if content then
                local lineCount = select(2, string.gsub(content, '\n', '\n')) + 1
                print("  Lines: " .. lineCount)
            else
                print("  Could not read file")
            end
        end
    end
end

-- Updated usage notes:
-- - All paths are now relative to pluginPath automatically
-- - Use "" or "." for the plugin root directory
-- - Use "subfolder" to access subdirectories
-- - Both functions automatically prepend pluginPath for security
-- - Trying to use "../" will be blocked by security checks
-- - Example: readfile("config.txt") reads pluginPath/config.txt
-- - Example: listfiles("data") lists files in pluginPath/data/