FROM node:lts-slim AS build
WORKDIR /src
RUN npm install -g @angular/cli

#COPY package*.json ./
#RUN npm ci

COPY . ./
WORKDIR "/src/application.client"
CMD ["/bin/sh","-c", "envsubst < src/environments/environment.ts > src/application.client/environments/environment.tmp.ts && mv src/application.client/environments/environment.tmp.ts src/application.client/environments/environment.ts"]
run npm install --force
RUN ng build --configuration=production

FROM nginx:stable AS final
EXPOSE 80
COPY --from=build src/application.client/dist/application.client/browser  /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/conf.d/default.conf