create view russian_cities as
Select city.id, city.name
FROM city, region
WHERE city.region_id=region.id and (region.country_id = 0 or region.country_id = -1);