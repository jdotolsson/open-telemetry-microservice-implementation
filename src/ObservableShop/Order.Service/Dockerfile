# # syntax=docker/dockerfile:1

# FROM golang:1.21

# WORKDIR /usr/src/app

# # pre-copy/cache go.mod for pre-downloading dependencies and only redownloading them in subsequent builds if they change
# COPY go.mod go.sum ./
# RUN go mod download && go mod verify

# COPY . .
# RUN go build -v -o /usr/local/bin/app/ ./...

# EXPOSE 8080

# CMD ["app"]

FROM golang:1.21 AS builder

RUN go version

COPY . "/usr/src/app"
WORKDIR "/usr/src/app"

COPY go.mod go.sum ./
RUN go mod download && go mod verify

RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build  -o /order-service

CMD ["/order-service"]

EXPOSE 8000



#########
# second stage to obtain a very small image
FROM scratch

COPY --from=builder /order-service .

EXPOSE 8000

CMD ["/order-service"]

