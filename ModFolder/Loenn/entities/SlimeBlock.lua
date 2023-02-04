local slimeBlock = {}

slimeBlock.name = "SaladimHelper/SlimeBlock"
slimeBlock.fillColor = { 58 / 255.0, 107 / 255.0, 57 / 255.0 }
slimeBlock.borderColor = { 1.0, 1.0, 1.0 }
slimeBlock.nodeLineRenderType = "line"
slimeBlock.nodeLimits = { 0, 1 }
slimeBlock.placements = {
    name = "saladimHelper_slimeBlock",
    data = {
        width = 8,
        height = 8,
        level = 1.1
    }
}
slimeBlock.fieldInformation = {
    level = {
        options = {
            ["None"] = 0.0,
            ["Sticky"] = 0.5,
            ["Small"] = 0.8,
            ["Normal"] = 1.1,
            ["Super"] = 1.3,
        },
        editable = false
    }
}

return slimeBlock
