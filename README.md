# photography-gallery-blazor-app

ASP.NET Core Blazor app for the display and download of photography images.

Images are stored in wwwroot/images directory, and only .jpg files are supported. Images are automatically processed on application start.

To deploy with docker:
```
docker build -t photo-gallery .
```
```
docker run -d -p 8080:80 -p 8443:443 photo-gallery
```
Or use the following docker-compose.yml:
```
version: '3'
services:
  photo-gallery-redis:
    image: redis:latest
  photo-gallery:
    image: photo-gallery
    volumes:
      - [path to persisted converted photos]:/app/wwwroot/images
      - [path to input photos]:/tmp/photography-gallery-input-images
    ports:
      - 80:80
    depends_on:
      - photo-gallery-redis
    restart: always

```
