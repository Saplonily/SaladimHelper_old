local miyRuleManagedField = {
    name = "SaladimHelper/MiyRuleManagedField",
    placements = {
        {
            name = "saladimHelper_miyRuleManagedField",
            data = {
                width = 80,
                height = 80,
                grid_size = 16
            }
        }
    }
}

function miyRuleManagedField.draw(room, entity, viewport)
    love.graphics.setColor(51.0 / 255, 98.0 / 255, 88.0 / 255, 0.3)
    love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    love.graphics.setColor(124.0 / 255, 251.0 / 255, 171.0 / 255, 0.5)
    local xx = entity.x;
    local yy = entity.y;
    while (xx < entity.x + entity.width) do
        love.graphics.line(xx, entity.y, xx, entity.y + entity.height)
        xx = xx + entity.grid_size
    end

    while (yy < entity.y + entity.height) do
        love.graphics.line(entity.x, yy, entity.x + entity.width, yy)
        yy = yy + entity.grid_size
    end
    love.graphics.setColor(255.0 / 255, 255.0 / 255, 255.0 / 255, 255.0 / 255)
end

return miyRuleManagedField
