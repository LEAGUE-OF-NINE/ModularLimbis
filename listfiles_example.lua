-- Example usage of the listfiles() function
-- This function securely lists files in directories within the plugin path

-- Example 1: List all files in the current plugin directory
print("=== Files in current directory ===")
local files = listfiles(".")
if files then
    for i = 1, #files do
        print("File " .. i .. ": " .. files[i])
    end
else
    print("No files found or directory doesn't exist")
end

-- Example 2: List files in a subdirectory
print("\n=== Files in 'scripts' subdirectory ===")
local scriptFiles = listfiles("scripts")
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
local luaFiles = listfiles(".")
if luaFiles then
    for i = 1, #luaFiles do
        local filename = luaFiles[i]
        if string.match(filename, "%.lua$") then
            print("Processing: " .. filename)
            local content = readfile(filename)
            if content then
                local lineCount = select(2, string.gsub(content, '\n', '\n')) + 1
                print("  Lines: " .. lineCount)
            else
                print("  Could not read file")
            end
        end
    end
end

-- Security notes:
-- - listfiles() only works within the plugin directory for security
-- - Trying to access "../" or absolute paths outside plugin dir will return nil
-- - Returns relative paths from the plugin directory
-- - Use with readfile() for complete file processing workflows