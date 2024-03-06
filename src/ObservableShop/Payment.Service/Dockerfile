FROM node:21

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

EXPOSE 8080

CMD ["npx", "ts-node", "--require", "./instrumentation.ts", "app.ts"]