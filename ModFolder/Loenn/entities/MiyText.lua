local miyText = {}

miyText.name = "SaladimHelper/MiyText"
miyText.placements = {
    name = "saladimHelper_miyText",
    data = {
        width = 8,
        height = 8,
        text = "madeline",
        texture = ""
    }
}

function miyText.draw(room, entity, viewport)
    love.graphics.setColor(51.0 / 255, 98.0 / 255, 88.0 / 255, 0.5)
    love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    love.graphics.setColor(124.0 / 255, 251.0 / 255, 171.0 / 255, 1)
    love.graphics.print(entity.text, entity.x, entity.y)
    love.graphics.setColor(1, 1, 1)
end

return miyText
