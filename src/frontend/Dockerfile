FROM node:22 as build

WORKDIR /app

COPY package*.json ./

RUN npm install

RUN npm install -g @angular/cli

COPY . .

RUN ng build --configuration=production

FROM nginx:latest

WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build app/dist/chat-bot-app/browser .

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
