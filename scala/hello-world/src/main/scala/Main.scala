
import scala.io.Source
import scala.util.{Failure, Success, Try}

object AdventOfcode extends App {
  Day1.solve
}

object Resource {
  def forDay(day: Int) = {
    Source.fromResource(s"day$day.txt").getLines()
  }
}

object Day1 {
  def solve = {
    val elfData = Resource.forDay(1)
    val elvesByCalories = elfData
      .foldLeft((0, List(0)))((indexAndList, maybeCalories) => {
        val currentValue = indexAndList._1
        val elfList = indexAndList._2
        Try(maybeCalories.toInt) match {
          case Success(value) => (currentValue + value, elfList)
          case Failure(_)     =>
            // turbo bad but im assuming a falure is a blank row - Ie. a new elf
            (0, elfList ++ List(currentValue))
        }
      })
      ._2

    val maxCalories = elvesByCalories.max
    val top3 = {
      elvesByCalories.sorted(Ordering.Int.reverse).take(3)
    }
    val top3Total = top3.sum

    println(s"maxCalories: $maxCalories\ntop3: $top3\ntop3Total: $top3Total")
  }
}