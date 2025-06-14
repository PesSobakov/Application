FROM node:lts-slim AS build
ARG API_URL
ENV API_URL="${API_URL}"
WORKDIR /src
RUN npm install -g @angular/cli

#COPY package*.json ./
#RUN npm ci

COPY . ./
WORKDIR "/src/application.client"
RUN sed "s/\$API_URL/${API_URL}/" ./src/environments/environment.ts > ./src/environments/environment.tmp.ts && mv ./src/environments/environment.tmp.ts ./src/environments/environment.ts
RUN npm install
RUN npm run build

FROM nginx:stable AS final
EXPOSE 80
COPY --from=build src/application.client/dist/application.client/browser  /usr/share/nginx/html
COPY ./nginx.conf /etc/nginx/conf.d/default.conf