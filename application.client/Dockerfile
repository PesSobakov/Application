FROM node:lts-slim AS build
WORKDIR /src
RUN npm install -g @angular/cli

COPY package*.json ./
RUN npm ci

COPY . ./
CMD ["/bin/sh","-c", "envsubst < src/environments/environment.ts > src/environments/environment.tmp.ts && mv src/environments/environment.tmp.ts src/environments/environment.ts"]
RUN ng build --configuration=production

FROM nginx:stable AS final
EXPOSE 80
COPY --from=build src/dist/angular-app/browser  /usr/share/nginx/html
