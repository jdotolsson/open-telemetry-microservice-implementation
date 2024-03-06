
$dockerFileTargets = @(
    @{
        dockerFileSource = "src\ObservableShop\Email.Service\Dockerfile"
        imageNameTag = "email.service:latest"
        buildContext = "src\ObservableShop" 
    }, 
    @{
        dockerFileSource = "src\ObservableShop\UpdateReceptionist.Service\Dockerfile"
        imageNameTag = "update_receptionist.service:latest"
        buildContext = "src\ObservableShop" 
    },   
    @{
        dockerFileSource = "src\ObservableShop\Shop.ApiGateway\Dockerfile"
        imageNameTag = "shop.apigateway:latest"
        buildContext = "src\ObservableShop" 
    },  
    @{
        dockerFileSource = "src\ObservableShop\Product.Service\Dockerfile"
        imageNameTag = "product.service:latest"
        buildContext = "src\ObservableShop\Product.Service" 
    },    
    @{
        dockerFileSource = "src\ObservableShop\Order.Service\Dockerfile"
        imageNameTag = "order.service:latest"
        buildContext = "src\ObservableShop\Order.Service" 
    },    
    @{
        dockerFileSource = "src\ObservableShop\Payment.Service\Dockerfile"
        imageNameTag = "payment.service:latest"
        buildContext = "src\ObservableShop\Payment.Service" 
    }
)

$location = Get-Location
$scriptRoot = $PSScriptRoot

$dockerFileTargets | ForEach-Object {    
    Write-Host "Building Docker Image $($_.imageNameTag) ..."    
    $dockerFileSource = Join-Path $scriptRoot $_.dockerFileSource
    $buildContext = Join-Path $scriptRoot $_.buildContext
    $imageNameTag = $_.imageNameTag
    
    docker build --pull --rm -f $dockerFileSource  -t $imageNameTag $buildContext
}

Set-Location $location